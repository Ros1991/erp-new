using ERP.Application.DTOs;

namespace ERP.Application.Interfaces.Services
{
    public interface IPayrollService
    {
        Task<List<PayrollOutputDTO>> GetAllAsync(long companyId);
        Task<PagedResult<PayrollOutputDTO>> GetPagedAsync(long companyId, PayrollFilterDTO filters);
        Task<PayrollOutputDTO> GetOneByIdAsync(long payrollId);
        Task<PayrollDetailedOutputDTO> GetDetailedByIdAsync(long payrollId);
        Task<PayrollOutputDTO> CreatePayrollAsync(long companyId, long currentUserId, PayrollInputDTO dto);
        Task<PayrollOutputDTO> UpdateByIdAsync(long payrollId, PayrollInputDTO dto, long currentUserId);
        Task<bool> DeleteByIdAsync(long payrollId);
        Task<PayrollDetailedOutputDTO> RecalculatePayrollAsync(long payrollId, long currentUserId);
        Task<PayrollEmployeeDetailedDTO> RecalculatePayrollEmployeeAsync(long payrollEmployeeId, long currentUserId);
        Task<PayrollItemDetailedDTO> UpdatePayrollItemAsync(long payrollItemId, UpdatePayrollItemDTO dto, long currentUserId);
        Task<PayrollEmployeeDetailedDTO> UpdateWorkedUnitsAsync(long payrollEmployeeId, UpdateWorkedUnitsDTO dto, long currentUserId);
        Task<PayrollEmployeeDetailedDTO> AddPayrollItemAsync(PayrollItemInputDTO dto, long currentUserId);
    }
}
