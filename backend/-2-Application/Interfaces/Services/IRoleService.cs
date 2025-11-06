using ERP.Application.DTOs;

namespace ERP.Application.Interfaces.Services
{
    public interface IRoleService
    {
        Task<List<RoleOutputDTO>> GetAllByCompanyAsync(long companyId);
        Task<RoleOutputDTO> GetOneByIdAsync(long roleId);
        Task<RoleOutputDTO> CreateAsync(RoleInputDTO dto, long companyId, long currentUserId);
        Task<RoleOutputDTO> UpdateByIdAsync(long roleId, RoleInputDTO dto, long currentUserId);
        Task<bool> DeleteByIdAsync(long roleId);
    }
}
