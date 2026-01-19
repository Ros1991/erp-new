using ERP.Application.DTOs;
using ERP.Application.Interfaces.Repositories;
using ERP.Domain.Entities;
using ERP.Infrastructure.Data;
using ERP.Infrastructure.Extensions;
using ERP.CrossCutting.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace ERP.Infrastructure.Repositories
{
    public class AccountPayableReceivableRepository : IAccountPayableReceivableRepository
    {
        private readonly ErpContext _context;

        public AccountPayableReceivableRepository(ErpContext context)
        {
            _context = context;
        }

        public async Task<List<AccountPayableReceivable>> GetAllAsync(long companyId)
        {
            return await _context.Set<AccountPayableReceivable>()
                .Where(a => a.CompanyId == companyId)
                .Include(x => x.SupplierCustomer)
                .ToListAsync();
        }

        public async Task<PagedResult<AccountPayableReceivable>> GetPagedAsync(long companyId, AccountPayableReceivableFilterDTO filters)
        {
            var query = _context.Set<AccountPayableReceivable>()
                .Where(a => a.CompanyId == companyId)
                .Include(x => x.SupplierCustomer)
                .AsQueryable();

            // Busca geral (Search)
            if (!string.IsNullOrWhiteSpace(filters.Search))
            {
                query = query.Where(x => 
                    x.Description.Contains(filters.Search) || 
                    x.Type.Contains(filters.Search));
            }

            // Contar total antes da paginação
            var total = await query.CountAsync();

            // ✅ Aplicar ordenação dinâmica
            if (!string.IsNullOrWhiteSpace(filters.OrderBy))
            {
                query = query.OrderByProperty(filters.OrderBy, filters.IsAscending);
            }
            else
            {
                query = query.OrderByDescending(x => x.CriadoEm); // Ordenação padrão
            }

            // Aplicar paginação
            var items = await query
                .Skip(filters.Skip)
                .Take(filters.PageSize)
                .ToListAsync();

            return new PagedResult<AccountPayableReceivable>(items, filters.Page, filters.PageSize, total);
        }

        public async Task<AccountPayableReceivable> GetOneByIdAsync(long accountPayableReceivableId)
        {
            return await _context.Set<AccountPayableReceivable>()
                .Include(x => x.SupplierCustomer)
                .Include(x => x.CostCenterDistributions)
                    .ThenInclude(d => d.CostCenter)
                .FirstOrDefaultAsync(x => x.AccountPayableReceivableId == accountPayableReceivableId);
        }

        public async Task<AccountPayableReceivable> CreateAsync(AccountPayableReceivable entity)
        {
            await _context.Set<AccountPayableReceivable>().AddAsync(entity);
            return entity;
        }

        public async Task<AccountPayableReceivable> UpdateByIdAsync(long accountPayableReceivableId, AccountPayableReceivable entity)
        {
            if (accountPayableReceivableId <= 0)
                throw new ValidationException(nameof(accountPayableReceivableId), "AccountPayableReceivableId deve ser maior que zero.");

            var existing = await _context.Set<AccountPayableReceivable>().FindAsync(accountPayableReceivableId);
            
            if (existing == null)
                throw new EntityNotFoundException("AccountPayableReceivable", accountPayableReceivableId);

            _context.Entry(existing).CurrentValues.SetValues(entity);
            return existing;
        }

        public async Task<bool> DeleteByIdAsync(long accountPayableReceivableId)
        {
            if (accountPayableReceivableId <= 0)
                throw new ValidationException(nameof(accountPayableReceivableId), "AccountPayableReceivableId deve ser maior que zero.");

            var existing = await _context.Set<AccountPayableReceivable>().FindAsync(accountPayableReceivableId);
            
            if (existing == null)
                throw new EntityNotFoundException("AccountPayableReceivable", accountPayableReceivableId);

            _context.Set<AccountPayableReceivable>().Remove(existing);
            return true;
        }

        // Métodos para relatórios
        public async Task<List<AccountPayableReceivable>> GetPendingAsync(long companyId, string? type = null)
        {
            var query = _context.Set<AccountPayableReceivable>()
                .Where(apr => apr.CompanyId == companyId && !apr.IsPaid)
                .Include(x => x.SupplierCustomer)
                .AsQueryable();

            if (!string.IsNullOrEmpty(type) && type != "Todos")
            {
                query = query.Where(apr => apr.Type == type);
            }

            return await query.OrderBy(apr => apr.DueDate).ToListAsync();
        }

        public async Task<List<AccountPayableReceivable>> GetPaidByDateRangeAsync(long companyId, DateTime startDate, DateTime endDate, string? type = null)
        {
            var query = _context.Set<AccountPayableReceivable>()
                .Where(apr => apr.CompanyId == companyId && 
                              apr.IsPaid && 
                              apr.DueDate >= startDate && 
                              apr.DueDate <= endDate)
                .Include(x => x.SupplierCustomer)
                .AsQueryable();

            if (!string.IsNullOrEmpty(type) && type != "Todos")
            {
                query = query.Where(apr => apr.Type == type);
            }

            return await query.OrderBy(apr => apr.DueDate).ToListAsync();
        }

        public async Task<List<AccountPayableReceivable>> GetPendingByDueDateRangeAsync(long companyId, DateTime startDate, DateTime endDate, string? type = null)
        {
            var query = _context.Set<AccountPayableReceivable>()
                .Where(apr => apr.CompanyId == companyId && 
                              !apr.IsPaid && 
                              apr.DueDate >= startDate && 
                              apr.DueDate <= endDate)
                .Include(x => x.SupplierCustomer)
                .AsQueryable();

            if (!string.IsNullOrEmpty(type) && type != "Todos")
            {
                query = query.Where(apr => apr.Type == type);
            }

            return await query.OrderBy(apr => apr.DueDate).ToListAsync();
        }
    }
}
