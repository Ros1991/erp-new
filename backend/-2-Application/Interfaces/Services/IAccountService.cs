using ERP.Application.DTOs;
using ERP.Application.DTOs.Base;

namespace ERP.Application.Interfaces.Services
{
    public interface IAccountService
    {
        Task<List<AccountOutputDTO>> GetAllAsync();
        Task<PagedResult<AccountOutputDTO>> GetPagedAsync(AccountFilterDTO filters);
        Task<AccountOutputDTO> GetOneByIdAsync(long AccountId);
        Task<AccountOutputDTO> CreateAsync(AccountInputDTO dto);
        Task<AccountOutputDTO> UpdateByIdAsync(long AccountId, AccountInputDTO dto);
        Task<bool> DeleteByIdAsync(long AccountId);
    }
}
