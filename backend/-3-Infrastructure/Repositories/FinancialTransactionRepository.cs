using ERP.Application.DTOs;
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
            IOrderedQueryable<FinancialTransaction> orderedQuery;
            if (!string.IsNullOrWhiteSpace(filters.OrderBy))
            {
                // Ordenação primária pelo campo solicitado, secundária por descrição
                if (filters.OrderBy.Equals("transactionDate", StringComparison.OrdinalIgnoreCase))
                {
                    orderedQuery = filters.IsAscending
                        ? query.OrderBy(x => x.TransactionDate).ThenBy(x => x.Description)
                        : query.OrderByDescending(x => x.TransactionDate).ThenBy(x => x.Description);
                }
                else
                {
                    // Para outros campos, usar ordenação dinâmica sem ThenBy
                    orderedQuery = (IOrderedQueryable<FinancialTransaction>)query.OrderByProperty(filters.OrderBy, filters.IsAscending);
                }
            }
            else
            {
                // Ordenação padrão: data decrescente, descrição ascendente
                orderedQuery = query.OrderByDescending(x => x.TransactionDate).ThenBy(x => x.Description);
            }
            query = orderedQuery;

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

        public async Task<FinancialTransaction> GetByLoanAdvanceIdAsync(long loanAdvanceId)
        {
            return await _context.Set<FinancialTransaction>()
                .Include(x => x.Account)
                .Include(x => x.TransactionCostCenterList)
                    .ThenInclude(tcc => tcc.CostCenter)
                .FirstOrDefaultAsync(x => x.LoanAdvanceId == loanAdvanceId);
        }

        public async Task<FinancialTransaction> GetByAccountPayableReceivableIdAsync(long accountPayableReceivableId)
        {
            return await _context.Set<FinancialTransaction>()
                .Include(x => x.Account)
                .Include(x => x.TransactionCostCenterList)
                    .ThenInclude(tcc => tcc.CostCenter)
                .FirstOrDefaultAsync(x => x.AccountPayableReceivableId == accountPayableReceivableId);
        }

        public async Task<FinancialTransaction> GetByPurchaseOrderIdAsync(long purchaseOrderId)
        {
            return await _context.Set<FinancialTransaction>()
                .Include(x => x.Account)
                .Include(x => x.TransactionCostCenterList)
                    .ThenInclude(tcc => tcc.CostCenter)
                .FirstOrDefaultAsync(x => x.PurchaseOrderId == purchaseOrderId);
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

        // Métodos para relatórios
        public async Task<List<FinancialTransaction>> GetByDateRangeAsync(long companyId, DateTime startDate, DateTime endDate)
        {
            return await _context.Set<FinancialTransaction>()
                .Where(t => t.CompanyId == companyId && 
                            t.TransactionDate >= startDate && 
                            t.TransactionDate <= endDate)
                .Include(x => x.Account)
                .Include(x => x.SupplierCustomer)
                .ToListAsync();
        }

        public async Task<List<FinancialTransaction>> GetByDateRangeWithCostCentersAsync(long companyId, DateTime startDate, DateTime endDate)
        {
            return await _context.Set<FinancialTransaction>()
                .Where(t => t.CompanyId == companyId && 
                            t.TransactionDate >= startDate && 
                            t.TransactionDate <= endDate)
                .Include(x => x.Account)
                .Include(x => x.SupplierCustomer)
                .Include(x => x.TransactionCostCenterList)
                    .ThenInclude(tcc => tcc.CostCenter)
                .ToListAsync();
        }

        public async Task<(long Entradas, long Saidas)> GetSumBeforeDateAsync(long companyId, DateTime date)
        {
            var result = await _context.Set<FinancialTransaction>()
                .Where(t => t.CompanyId == companyId && t.TransactionDate < date)
                .GroupBy(t => 1)
                .Select(g => new
                {
                    Entradas = g.Where(t => t.Type == "Entrada").Sum(t => t.Amount),
                    Saidas = g.Where(t => t.Type == "Saída").Sum(t => t.Amount)
                })
                .FirstOrDefaultAsync();

            return result != null ? (result.Entradas, result.Saidas) : (0, 0);
        }

        public async Task<(long Entradas, long Saidas)> GetSumUpToDateAsync(long companyId, DateTime date)
        {
            var result = await _context.Set<FinancialTransaction>()
                .Where(t => t.CompanyId == companyId && t.TransactionDate <= date)
                .GroupBy(t => 1)
                .Select(g => new
                {
                    Entradas = g.Where(t => t.Type == "Entrada").Sum(t => t.Amount),
                    Saidas = g.Where(t => t.Type == "Saída").Sum(t => t.Amount)
                })
                .FirstOrDefaultAsync();

            return result != null ? (result.Entradas, result.Saidas) : (0, 0);
        }
    }
}
