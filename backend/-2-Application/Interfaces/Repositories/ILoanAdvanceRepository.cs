using ERP.Application.DTOs;
using ERP.Domain.Entities;

namespace ERP.Application.Interfaces.Repositories
{
    public interface ILoanAdvanceRepository
    {
        Task<List<LoanAdvance>> GetAllAsync(long companyId);
        Task<PagedResult<LoanAdvance>> GetPagedAsync(long companyId, LoanAdvanceFilterDTO filters);
        Task<LoanAdvance> GetOneByIdAsync(long loanAdvanceId);
        Task<List<LoanAdvance>> GetPendingLoansByEmployeeAsync(long employeeId, DateTime? referenceDate = null);
        Task<LoanAdvance> CreateAsync(LoanAdvance entity);
        Task<LoanAdvance> UpdateByIdAsync(long loanAdvanceId, LoanAdvance entity);
        Task<bool> DeleteByIdAsync(long loanAdvanceId);
    }
}
