using ERP.Application.DTOs;
using ERP.Domain.Entities;

namespace ERP.Application.Interfaces.Repositories
{
    public interface IPurchaseOrderRepository
    {
        Task<List<PurchaseOrder>> GetAllAsync(long companyId);
        Task<PagedResult<PurchaseOrder>> GetPagedAsync(long companyId, PurchaseOrderFilterDTO filters);
        Task<PurchaseOrder> GetOneByIdAsync(long purchaseOrderId);
        Task<PurchaseOrder> CreateAsync(PurchaseOrder entity);
        Task<PurchaseOrder> UpdateByIdAsync(long purchaseOrderId, PurchaseOrder entity);
        Task<bool> DeleteByIdAsync(long purchaseOrderId);
    }
}
