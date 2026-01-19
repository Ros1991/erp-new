using ERP.Application.DTOs;
using ERP.Application.Interfaces.Repositories;
using ERP.Domain.Entities;
using ERP.Infrastructure.Data;
using ERP.Infrastructure.Extensions;
using ERP.CrossCutting.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace ERP.Infrastructure.Repositories
{
    public class LoanAdvanceRepository : ILoanAdvanceRepository
    {
        private readonly ErpContext _context;

        public LoanAdvanceRepository(ErpContext context)
        {
            _context = context;
        }

        public async Task<List<LoanAdvance>> GetAllAsync(long companyId)
        {
            return await _context.Set<LoanAdvance>()
                .Include(x => x.Employee)
                .Where(a => a.Employee.CompanyId == companyId)
                .ToListAsync();
        }

        public async Task<PagedResult<LoanAdvance>> GetPagedAsync(long companyId, LoanAdvanceFilterDTO filters)
        {
            var query = _context.Set<LoanAdvance>()
                .Include(x => x.Employee)
                .Where(a => a.Employee.CompanyId == companyId)
                .AsQueryable();

            // Busca geral (Search)
            if (!string.IsNullOrWhiteSpace(filters.Search))
            {
                query = query.Where(x => 
                    x.DiscountSource.Contains(filters.Search) ||
                    x.Employee.Nickname.Contains(filters.Search) ||
                    (x.Description != null && x.Description.Contains(filters.Search)));
            }

            // Filtro de apenas em aberto
            if (filters.OnlyOpen == true)
            {
                query = query.Where(x => !x.IsFullyPaid);
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

            return new PagedResult<LoanAdvance>(items, filters.Page, filters.PageSize, total);
        }

        public async Task<LoanAdvance> GetOneByIdAsync(long loanAdvanceId)
        {
            return await _context.Set<LoanAdvance>()
                .Include(x => x.Employee)
                .FirstOrDefaultAsync(x => x.LoanAdvanceId == loanAdvanceId);
        }

        public async Task<List<LoanAdvance>> GetPendingLoansByEmployeeAsync(long employeeId, DateTime? referenceDate = null)
        {
            var query = _context.Set<LoanAdvance>()
                .Where(la => la.EmployeeId == employeeId &&
                            la.IsApproved == true &&
                            la.IsFullyPaid == false);

            // Se há data de referência (período final da folha), não incluir empréstimos
            // cuja data de início seja posterior, pois ainda não devem começar a ser descontados
            if (referenceDate.HasValue)
            {
                query = query.Where(la => la.StartDate <= referenceDate.Value);
            }

            return await query
                .OrderBy(la => la.StartDate)
                .ToListAsync();
        }

        public async Task<LoanAdvance> CreateAsync(LoanAdvance entity)
        {
            await _context.Set<LoanAdvance>().AddAsync(entity);
            return entity;
        }

        public async Task<LoanAdvance> UpdateByIdAsync(long loanAdvanceId, LoanAdvance entity)
        {
            if (loanAdvanceId <= 0)
                throw new ValidationException(nameof(loanAdvanceId), "LoanAdvanceId deve ser maior que zero.");

            var existing = await _context.Set<LoanAdvance>().FindAsync(loanAdvanceId);
            
            if (existing == null)
                throw new EntityNotFoundException("LoanAdvance", loanAdvanceId);

            _context.Entry(existing).CurrentValues.SetValues(entity);
            return existing;
        }

        public async Task<bool> DeleteByIdAsync(long loanAdvanceId)
        {
            if (loanAdvanceId <= 0)
                throw new ValidationException(nameof(loanAdvanceId), "LoanAdvanceId deve ser maior que zero.");

            var existing = await _context.Set<LoanAdvance>().FindAsync(loanAdvanceId);
            
            if (existing == null)
                throw new EntityNotFoundException("LoanAdvance", loanAdvanceId);

            _context.Set<LoanAdvance>().Remove(existing);
            return true;
        }
    }
}
