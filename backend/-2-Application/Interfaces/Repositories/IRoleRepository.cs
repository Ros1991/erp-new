using ERP.Domain.Entities;

namespace ERP.Application.Interfaces.Repositories
{
    public interface IRoleRepository
    {
        Task<List<Role>> GetAllAsync(long companyId);
        Task<Role> GetOneByIdAsync(long roleId);
        Task<Role> CreateAsync(Role entity);
        Task<Role> UpdateByIdAsync(long roleId, Role entity);
        Task<bool> DeleteByIdAsync(long roleId);
    }
}
