using ERP.Application.DTOs;

namespace ERP.Application.Interfaces.Services
{
    public interface IPayrollService
    {
        Task<List<PayrollOutputDTO>> GetAllAsync(long companyId);
        Task<PagedResult<PayrollOutputDTO>> GetPagedAsync(long companyId, PayrollFilterDTO filters);
        Task<PayrollOutputDTO> GetOneByIdAsync(long payrollId);
        Task<PayrollOutputDTO> CreatePayrollAsync(long companyId, long currentUserId, PayrollInputDTO dto);
        Task<PayrollOutputDTO> UpdateByIdAsync(long payrollId, PayrollInputDTO dto, long currentUserId);
        Task<bool> DeleteByIdAsync(long payrollId);
    }
}
