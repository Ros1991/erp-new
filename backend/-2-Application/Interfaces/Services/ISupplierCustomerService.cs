using ERP.Application.DTOs;
using ERP.Application.DTOs.Base;

namespace ERP.Application.Interfaces.Services
{
    public interface ISupplierCustomerService
    {
        Task<List<SupplierCustomerOutputDTO>> GetAllAsync(long companyId);
        Task<PagedResult<SupplierCustomerOutputDTO>> GetPagedAsync(long companyId, SupplierCustomerFilterDTO filters);
        Task<SupplierCustomerOutputDTO> GetOneByIdAsync(long supplierCustomerId);
        Task<SupplierCustomerOutputDTO> CreateAsync(SupplierCustomerInputDTO dto, long companyId, long currentUserId);
        Task<SupplierCustomerOutputDTO> UpdateByIdAsync(long supplierCustomerId, SupplierCustomerInputDTO dto, long currentUserId);
        Task<bool> DeleteByIdAsync(long supplierCustomerId);
    }
}
