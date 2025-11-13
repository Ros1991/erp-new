using ERP.Application.DTOs;
using ERP.Application.DTOs.Base;
using ERP.Application.Interfaces.Repositories;
using ERP.Domain.Entities;
using ERP.Infrastructure.Data;
using ERP.Infrastructure.Extensions;
using ERP.CrossCutting.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace ERP.Infrastructure.Repositories
{
    public class FinancialTransactionRepository : IFinancialTransactionRepository
    {
        private readonly ErpContext _context;

        public FinancialTransactionRepository(ErpContext context)
        {
            _context = context;
        }

        public async Task<List<FinancialTransaction>> GetAllAsync(long companyId)
        {
            return await _context.Set<FinancialTransaction>()
                .Where(a => a.CompanyId == companyId)
                .Include(x => x.Account)
                .Include(x => x.SupplierCustomer)
                .ToListAsync();
        }

        public async Task<PagedResult<FinancialTransaction>> GetPagedAsync(long companyId, FinancialTransactionFilterDTO filters)
        {
            var query = _context.Set<FinancialTransaction>()
                .Where(a => a.CompanyId == companyId)
                .Include(x => x.Account)
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

            // Aplicar ordenação dinâmica
            if (!string.IsNullOrWhiteSpace(filters.OrderBy))
            {
                query = query.OrderByProperty(filters.OrderBy, filters.IsAscending);
            }
            else
            {
                query = query.OrderByDescending(x => x.TransactionDate); // Ordenação padrão
            }

            // Aplicar paginação
            var items = await query
                .Skip(filters.Skip)
                .Take(filters.PageSize)
                .ToListAsync();

            return new PagedResult<FinancialTransaction>(items, filters.Page, filters.PageSize, total);
        }

        public async Task<FinancialTransaction> GetOneByIdAsync(long financialTransactionId)
        {
            return await _context.Set<FinancialTransaction>()
                .Include(x => x.Account)
                .Include(x => x.SupplierCustomer)
                .Include(x => x.TransactionCostCenterList)
                    .ThenInclude(tcc => tcc.CostCenter)
                .FirstOrDefaultAsync(x => x.FinancialTransactionId == financialTransactionId);
        }

        public async Task<FinancialTransaction> CreateAsync(FinancialTransaction entity)
        {
            await _context.Set<FinancialTransaction>().AddAsync(entity);
            return entity;
        }

        public async Task<FinancialTransaction> UpdateByIdAsync(long financialTransactionId, FinancialTransaction entity)
        {
            if (financialTransactionId <= 0)
                throw new ValidationException(nameof(financialTransactionId), "FinancialTransactionId deve ser maior que zero.");

            var existing = await _context.Set<FinancialTransaction>().FindAsync(financialTransactionId);
            
            if (existing == null)
                throw new EntityNotFoundException("FinancialTransaction", financialTransactionId);

            _context.Entry(existing).CurrentValues.SetValues(entity);
            return existing;
        }

        public async Task<bool> DeleteByIdAsync(long financialTransactionId)
        {
            if (financialTransactionId <= 0)
                throw new ValidationException(nameof(financialTransactionId), "FinancialTransactionId deve ser maior que zero.");

            var existing = await _context.Set<FinancialTransaction>().FindAsync(financialTransactionId);
            
            if (existing == null)
                throw new EntityNotFoundException("FinancialTransaction", financialTransactionId);

            _context.Set<FinancialTransaction>().Remove(existing);
            return true;
        }
    }
}
