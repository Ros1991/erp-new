using ERP.Application.DTOs;
using ERP.Domain.Entities;

namespace ERP.Application.Interfaces.Repositories
{
    public interface IFinancialTransactionRepository
    {
        Task<List<FinancialTransaction>> GetAllAsync(long companyId);
        Task<PagedResult<FinancialTransaction>> GetPagedAsync(long companyId, FinancialTransactionFilterDTO filters);
        Task<FinancialTransaction> GetOneByIdAsync(long financialTransactionId);
        Task<FinancialTransaction> GetByLoanAdvanceIdAsync(long loanAdvanceId);
        Task<FinancialTransaction> GetByAccountPayableReceivableIdAsync(long accountPayableReceivableId);
        Task<FinancialTransaction> GetByPurchaseOrderIdAsync(long purchaseOrderId);
        Task<FinancialTransaction> CreateAsync(FinancialTransaction entity);
        Task<FinancialTransaction> UpdateByIdAsync(long financialTransactionId, FinancialTransaction entity);
        Task<bool> DeleteByIdAsync(long financialTransactionId);
        
        // Métodos para relatórios
        Task<List<FinancialTransaction>> GetByDateRangeAsync(long companyId, DateTime startDate, DateTime endDate);
        Task<List<FinancialTransaction>> GetByDateRangeWithCostCentersAsync(long companyId, DateTime startDate, DateTime endDate);
        Task<(long Entradas, long Saidas)> GetSumBeforeDateAsync(long companyId, DateTime date);
        Task<(long Entradas, long Saidas)> GetSumUpToDateAsync(long companyId, DateTime date);
    }
}
