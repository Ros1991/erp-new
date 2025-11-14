using ERP.Application.DTOs;

namespace ERP.Application.Interfaces.Services
{
    public interface ILoanAdvanceService
    {
        Task<List<LoanAdvanceOutputDTO>> GetAllAsync(long companyId);
        Task<PagedResult<LoanAdvanceOutputDTO>> GetPagedAsync(long companyId, LoanAdvanceFilterDTO filters);
        Task<LoanAdvanceOutputDTO> GetOneByIdAsync(long loanAdvanceId);
        Task<LoanAdvanceOutputDTO> CreateAsync(LoanAdvanceInputDTO dto, long companyId, long currentUserId);
        Task<LoanAdvanceOutputDTO> UpdateByIdAsync(long loanAdvanceId, LoanAdvanceInputDTO dto, long currentUserId);
        Task<bool> DeleteByIdAsync(long loanAdvanceId, long companyId);
    }
}
