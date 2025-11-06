using ERP.Application.DTOs;
using ERP.Application.DTOs.Base;

namespace ERP.Application.Interfaces.Services
{
    public interface IAccountService
    {
        Task<List<AccountOutputDTO>> GetAllAsync(long companyId);
        Task<PagedResult<AccountOutputDTO>> GetPagedAsync(long companyId, AccountFilterDTO filters);
        Task<AccountOutputDTO> GetOneByIdAsync(long AccountId);
        Task<AccountOutputDTO> CreateAsync(AccountInputDTO dto, long companyId, long currentUserId);
        Task<AccountOutputDTO> UpdateByIdAsync(long AccountId, AccountInputDTO dto, long currentUserId);
        Task<bool> DeleteByIdAsync(long AccountId);
    }
}
