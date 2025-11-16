using ERP.Application.DTOs;

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
        
        // Métodos para associação de usuário
        Task<UserSearchResultDTO> SearchUserForEmployeeAsync(long employeeId, long companyId);
        Task<EmployeeOutputDTO> AssociateUserAsync(long employeeId, AssociateUserDTO dto, long companyId, long currentUserId);
        Task<EmployeeOutputDTO> CreateAndAssociateUserAsync(long employeeId, CreateUserAndAssociateDTO dto, long companyId, long currentUserId);
        Task<EmployeeOutputDTO> DisassociateUserAsync(long employeeId, DisassociateUserDTO dto, long companyId, long currentUserId);
    }
}
