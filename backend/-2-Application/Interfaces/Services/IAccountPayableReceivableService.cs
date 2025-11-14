using ERP.Application.DTOs;

namespace ERP.Application.Interfaces.Services
{
    public interface IAccountPayableReceivableService
    {
        Task<List<AccountPayableReceivableOutputDTO>> GetAllAsync(long companyId);
        Task<PagedResult<AccountPayableReceivableOutputDTO>> GetPagedAsync(long companyId, AccountPayableReceivableFilterDTO filters);
        Task<AccountPayableReceivableOutputDTO> GetOneByIdAsync(long accountPayableReceivableId);
        Task<AccountPayableReceivableOutputDTO> CreateAsync(AccountPayableReceivableInputDTO dto, long companyId, long currentUserId);
        Task<AccountPayableReceivableOutputDTO> UpdateByIdAsync(long accountPayableReceivableId, AccountPayableReceivableInputDTO dto, long currentUserId);
        Task<bool> DeleteByIdAsync(long accountPayableReceivableId);
    }
}
