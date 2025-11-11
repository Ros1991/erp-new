using ERP.Application.DTOs;
using ERP.Application.DTOs.Base;
using ERP.Domain.Entities;

namespace ERP.Application.Interfaces.Repositories
{
    public interface IRoleRepository
    {
        Task<List<Role>> GetAllAsync(long companyId);
        Task<PagedResult<Role>> GetPagedAsync(long companyId, RoleFilterDTO filters);
        Task<Role> GetOneByIdAsync(long roleId);
        Task<Role> CreateAsync(Role entity);
        Task<Role> UpdateByIdAsync(long roleId, Role entity);
        Task<bool> DeleteByIdAsync(long roleId);
    }
}
