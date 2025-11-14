using ERP.Application.DTOs;
using ERP.Domain.Entities;

namespace ERP.Application.Interfaces.Repositories
{
    public interface ISupplierCustomerRepository
    {
        Task<List<SupplierCustomer>> GetAllAsync(long companyId);
        Task<PagedResult<SupplierCustomer>> GetPagedAsync(long companyId, SupplierCustomerFilterDTO filters);
        Task<SupplierCustomer> GetOneByIdAsync(long supplierCustomerId);
        Task<SupplierCustomer> CreateAsync(SupplierCustomer entity);
        Task<SupplierCustomer> UpdateByIdAsync(long supplierCustomerId, SupplierCustomer entity);
        Task<bool> DeleteByIdAsync(long supplierCustomerId);
    }
}
