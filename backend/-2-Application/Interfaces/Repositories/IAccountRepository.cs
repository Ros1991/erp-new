using ERP.Application.DTOs;
using ERP.Domain.Entities;

namespace ERP.Application.Interfaces.Repositories
{
    public interface IAccountRepository
    {
        Task<List<Account>> GetAllAsync(long companyId);
        Task<PagedResult<Account>> GetPagedAsync(long companyId, AccountFilterDTO filters);
        Task<Account> GetOneByIdAsync(long AccountId);
        Task<Account> CreateAsync(Account entity);
        Task<Account> UpdateByIdAsync(long AccountId, Account entity);
        Task<bool> DeleteByIdAsync(long AccountId);
        
        // Métodos para relatórios
        Task<long> GetTotalInitialBalanceAsync(long companyId);
        Task<int> CountAsync(long companyId);
    }
}
