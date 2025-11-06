using CAU.Application.DTOs;
using CAU.Application.DTOs.Base;
using CAU.Domain.Entities;

namespace CAU.Application.Interfaces.Repositories
{
    public interface IAccountRepository
    {
        Task<List<Account>> GetAllAsync();
        Task<PagedResult<Account>> GetPagedAsync(AccountFilterDTO filters);
        Task<Account> GetOneByIdAsync(long AccountId);
        Task<Account> CreateAsync(Account entity);
        Task<Account> UpdateByIdAsync(long AccountId, Account entity);
        Task<bool> DeleteByIdAsync(long AccountId);
    }
}
