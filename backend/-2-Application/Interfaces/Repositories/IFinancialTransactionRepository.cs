using ERP.Application.DTOs;
using ERP.Domain.Entities;

namespace ERP.Application.Interfaces.Repositories
{
    public interface IFinancialTransactionRepository
    {
        Task<List<FinancialTransaction>> GetAllAsync(long companyId);
        Task<PagedResult<FinancialTransaction>> GetPagedAsync(long companyId, FinancialTransactionFilterDTO filters);
        Task<FinancialTransaction> GetOneByIdAsync(long financialTransactionId);
        Task<FinancialTransaction> CreateAsync(FinancialTransaction entity);
        Task<FinancialTransaction> UpdateByIdAsync(long financialTransactionId, FinancialTransaction entity);
        Task<bool> DeleteByIdAsync(long financialTransactionId);
    }
}
