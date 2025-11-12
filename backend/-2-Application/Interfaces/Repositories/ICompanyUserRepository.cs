using ERP.Application.DTOs;
using ERP.Application.DTOs.Base;
using ERP.Domain.Entities;

namespace ERP.Application.Interfaces.Repositories
{
    public interface ICompanyUserRepository
    {
        Task<List<CompanyUser>> GetAllAsync(long companyId);
        Task<PagedResult<CompanyUser>> GetPagedAsync(long companyId, CompanyUserFilterDTO filters);
        Task<CompanyUser> GetByUserAndCompanyAsync(long userId, long companyId);
        Task<CompanyUser> GetOneByIdAsync(long companyUserId);
        Task<Role> GetUserRoleInCompanyAsync(long userId, long companyId);
        Task<bool> UserHasAccessToCompanyAsync(long userId, long companyId);
        Task<CompanyUser> CreateAsync(CompanyUser entity);
        Task<CompanyUser> UpdateByIdAsync(long companyUserId, CompanyUser entity);
        Task<bool> DeleteByIdAsync(long companyUserId);
    }
}
