using ERP.Application.DTOs;

namespace ERP.Application.Interfaces.Services
{
    public interface IRoleService
    {
        Task<List<RoleOutputDTO>> GetAllAsync(long companyId);
        Task<PagedResult<RoleOutputDTO>> GetPagedAsync(long companyId, RoleFilterDTO filters);
        Task<RoleOutputDTO> GetOneByIdAsync(long roleId);
        Task<RoleOutputDTO> CreateAsync(RoleInputDTO dto, long companyId, long currentUserId);
        Task<RoleOutputDTO> UpdateByIdAsync(long roleId, RoleInputDTO dto, long currentUserId);
        Task<bool> DeleteByIdAsync(long roleId);
    }
}
