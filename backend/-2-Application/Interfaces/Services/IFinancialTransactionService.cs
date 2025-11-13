using ERP.Application.DTOs;
using ERP.Application.DTOs.Base;

namespace ERP.Application.Interfaces.Services
{
    public interface IFinancialTransactionService
    {
        Task<List<FinancialTransactionOutputDTO>> GetAllAsync(long companyId);
        Task<PagedResult<FinancialTransactionOutputDTO>> GetPagedAsync(long companyId, FinancialTransactionFilterDTO filters);
        Task<FinancialTransactionOutputDTO> GetOneByIdAsync(long financialTransactionId);
        Task<FinancialTransactionOutputDTO> CreateAsync(FinancialTransactionInputDTO dto, long companyId, long currentUserId);
        Task<FinancialTransactionOutputDTO> UpdateByIdAsync(long financialTransactionId, FinancialTransactionInputDTO dto, long currentUserId);
        Task<bool> DeleteByIdAsync(long financialTransactionId);
    }
}
