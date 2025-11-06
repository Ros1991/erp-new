using ERP.Application.DTOs;

namespace ERP.Application.Interfaces.Services
{
    public interface ICompanyUserService
    {
        Task<List<CompanyUserOutputDTO>> GetAllByCompanyAsync(long companyId);
        Task<CompanyUserOutputDTO> GetOneByIdAsync(long companyUserId);
        Task<CompanyUserOutputDTO> AddUserToCompanyAsync(CompanyUserInputDTO dto, long companyId, long currentUserId);
        Task<CompanyUserOutputDTO> UpdateUserRoleAsync(long companyUserId, CompanyUserInputDTO dto, long currentUserId);
        Task<bool> RemoveUserFromCompanyAsync(long companyUserId);
    }
}
