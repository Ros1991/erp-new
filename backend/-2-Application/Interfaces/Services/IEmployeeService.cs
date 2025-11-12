using ERP.Application.DTOs.Employee;
using ERP.Application.DTOs.Base;

namespace ERP.Application.Interfaces.Services
{
    public interface IEmployeeService
    {
        Task<List<EmployeeOutputDTO>> GetAllAsync(long companyId);
        Task<PagedResult<EmployeeOutputDTO>> GetPagedAsync(long companyId, EmployeeFilterDTO filters);
        Task<EmployeeOutputDTO> GetOneByIdAsync(long employeeId);
        Task<EmployeeOutputDTO> CreateAsync(EmployeeInputDTO dto, long companyId, long currentUserId);
        Task<EmployeeOutputDTO> UpdateByIdAsync(long employeeId, EmployeeInputDTO dto, long currentUserId);
        Task<bool> DeleteByIdAsync(long employeeId);
    }
}
