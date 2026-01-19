using ERP.Application.DTOs;
using ERP.Domain.Entities;

namespace ERP.Application.Interfaces.Repositories
{
    public interface IAccountPayableReceivableRepository
    {
        Task<List<AccountPayableReceivable>> GetAllAsync(long companyId);
        Task<PagedResult<AccountPayableReceivable>> GetPagedAsync(long companyId, AccountPayableReceivableFilterDTO filters);
        Task<AccountPayableReceivable> GetOneByIdAsync(long accountPayableReceivableId);
        Task<AccountPayableReceivable> CreateAsync(AccountPayableReceivable entity);
        Task<AccountPayableReceivable> UpdateByIdAsync(long accountPayableReceivableId, AccountPayableReceivable entity);
        Task<bool> DeleteByIdAsync(long accountPayableReceivableId);
        
        // Métodos para relatórios
        Task<List<AccountPayableReceivable>> GetPendingAsync(long companyId, string? type = null);
        Task<List<AccountPayableReceivable>> GetPaidByDateRangeAsync(long companyId, DateTime startDate, DateTime endDate, string? type = null);
        Task<List<AccountPayableReceivable>> GetPendingByDueDateRangeAsync(long companyId, DateTime startDate, DateTime endDate, string? type = null);
    }
}
