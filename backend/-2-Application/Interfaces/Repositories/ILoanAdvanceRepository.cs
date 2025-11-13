using ERP.Application.DTOs;
using ERP.Application.DTOs.Base;
using ERP.Domain.Entities;

namespace ERP.Application.Interfaces.Repositories
{
    public interface ILoanAdvanceRepository
    {
        Task<List<LoanAdvance>> GetAllAsync(long companyId);
        Task<PagedResult<LoanAdvance>> GetPagedAsync(long companyId, LoanAdvanceFilterDTO filters);
        Task<LoanAdvance> GetOneByIdAsync(long loanAdvanceId);
        Task<LoanAdvance> CreateAsync(LoanAdvance entity);
        Task<LoanAdvance> UpdateByIdAsync(long loanAdvanceId, LoanAdvance entity);
        Task<bool> DeleteByIdAsync(long loanAdvanceId);
    }
}
