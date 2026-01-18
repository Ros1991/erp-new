using ERP.Application.DTOs;
using ERP.Application.Interfaces.Repositories;
using ERP.Domain.Entities;
using ERP.Infrastructure.Data;
using ERP.Infrastructure.Extensions;
using ERP.CrossCutting.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace ERP.Infrastructure.Repositories
{
    public class PayrollRepository : IPayrollRepository
    {
        private readonly ErpContext _context;

        public PayrollRepository(ErpContext context)
        {
            _context = context;
        }

        public async Task<List<Payroll>> GetAllAsync(long companyId)
        {
            return await _context.Set<Payroll>()
                .Where(p => p.CompanyId == companyId)
                .OrderByDescending(p => p.PeriodEndDate)
                .ToListAsync();
        }

        public async Task<PagedResult<Payroll>> GetPagedAsync(long companyId, PayrollFilterDTO filters)
        {
            var query = _context.Set<Payroll>()
                .Include(p => p.Company)
                .Where(p => p.CompanyId == companyId)
                .AsQueryable();

            // Filtro por data de início do período
            if (filters.PeriodStartDate.HasValue)
            {
                query = query.Where(p => p.PeriodStartDate >= filters.PeriodStartDate.Value);
            }

            // Filtro por data de fim do período
            if (filters.PeriodEndDate.HasValue)
            {
                query = query.Where(p => p.PeriodEndDate <= filters.PeriodEndDate.Value);
            }

            // Filtro por status de fechamento
            if (filters.IsClosed.HasValue)
            {
                query = query.Where(p => p.IsClosed == filters.IsClosed.Value);
            }

            // Busca geral (Search) - busca no período ou nas observações
            if (!string.IsNullOrWhiteSpace(filters.Search))
            {
                var searchLower = filters.Search.ToLower();
                query = query.Where(p => 
                    (p.Notes != null && p.Notes.ToLower().Contains(searchLower)) ||
                    p.PeriodStartDate.ToString().Contains(filters.Search) ||
                    p.PeriodEndDate.ToString().Contains(filters.Search));
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
                // Ordenação padrão: mais recentes primeiro
                query = query.OrderByDescending(p => p.PeriodEndDate);
            }

            // Aplicar paginação
            var items = await query
                .Skip(filters.Skip)
                .Take(filters.PageSize)
                .ToListAsync();

            return new PagedResult<Payroll>(items, filters.Page, filters.PageSize, total);
        }

        public async Task<Payroll> GetOneByIdAsync(long payrollId)
        {
            return await _context.Set<Payroll>().FindAsync(payrollId);
        }

        public async Task<Payroll> GetOneByIdWithIncludesAsync(long payrollId)
        {
            return await _context.Set<Payroll>()
                .Include(p => p.Company)
                .Include(p => p.PayrollEmployeeList)
                    .ThenInclude(pe => pe.Employee)
                .Include(p => p.PayrollEmployeeList)
                    .ThenInclude(pe => pe.Contract)
                .Include(p => p.PayrollEmployeeList)
                    .ThenInclude(pe => pe.PayrollItemList)
                .FirstOrDefaultAsync(p => p.PayrollId == payrollId);
        }

        public async Task<Payroll> GetByCompanyAndPeriodAsync(long companyId, DateTime startDate, DateTime endDate)
        {
            return await _context.Set<Payroll>()
                .FirstOrDefaultAsync(p => 
                    p.CompanyId == companyId &&
                    p.PeriodStartDate == startDate &&
                    p.PeriodEndDate == endDate);
        }

        public async Task<Payroll> GetLastPayrollAsync(long companyId)
        {
            return await _context.Set<Payroll>()
                .Where(p => p.CompanyId == companyId)
                .OrderByDescending(p => p.PeriodEndDate)
                .FirstOrDefaultAsync();
        }

        public async Task<Payroll?> GetLastClosedPayrollByCompanyAsync(long companyId)
        {
            return await _context.Set<Payroll>()
                .Where(p => p.CompanyId == companyId && p.IsClosed)
                .OrderByDescending(p => p.PeriodEndDate)
                .FirstOrDefaultAsync();
        }

        public async Task<Payroll?> GetOpenPayrollByCompanyAsync(long companyId)
        {
            return await _context.Set<Payroll>()
                .Where(p => p.CompanyId == companyId && !p.IsClosed)
                .OrderByDescending(p => p.PeriodEndDate)
                .FirstOrDefaultAsync();
        }

        public async Task<int> GetEmployeeCountAsync(long payrollId)
        {
            return await _context.Set<PayrollEmployee>()
                .Where(pe => pe.PayrollId == payrollId)
                .CountAsync();
        }

        public async Task<Payroll> CreateAsync(Payroll entity)
        {
            await _context.Set<Payroll>().AddAsync(entity);
            return entity;
        }

        public async Task<Payroll> UpdateByIdAsync(long payrollId, Payroll entity)
        {
            if (payrollId <= 0)
                throw new ValidationException(nameof(payrollId), "PayrollId deve ser maior que zero.");

            var existing = await _context.Set<Payroll>().FindAsync(payrollId);
            
            if (existing == null)
                throw new EntityNotFoundException("Payroll", payrollId);

            _context.Entry(existing).CurrentValues.SetValues(entity);
            return existing;
        }

        public async Task<bool> DeleteByIdAsync(long payrollId)
        {
            if (payrollId <= 0)
                throw new ValidationException(nameof(payrollId), "PayrollId deve ser maior que zero.");

            var existing = await _context.Set<Payroll>().FindAsync(payrollId);
            
            if (existing == null)
                throw new EntityNotFoundException("Payroll", payrollId);

            _context.Set<Payroll>().Remove(existing);
            return true;
        }
    }
}
