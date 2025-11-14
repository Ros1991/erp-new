using ERP.Application.DTOs;

namespace ERP.Application.Interfaces.Services
{
    public interface IPurchaseOrderService
    {
        Task<List<PurchaseOrderOutputDTO>> GetAllAsync(long companyId);
        Task<PagedResult<PurchaseOrderOutputDTO>> GetPagedAsync(long companyId, PurchaseOrderFilterDTO filters);
        Task<PurchaseOrderOutputDTO> GetOneByIdAsync(long purchaseOrderId);
        Task<PurchaseOrderOutputDTO> CreateAsync(PurchaseOrderInputDTO dto, long companyId, long currentUserId);
        Task<PurchaseOrderOutputDTO> UpdateByIdAsync(long purchaseOrderId, PurchaseOrderInputDTO dto, long currentUserId);
        Task<bool> DeleteByIdAsync(long purchaseOrderId);
    }
}
