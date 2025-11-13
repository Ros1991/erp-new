using ERP.Application.DTOs;
using ERP.Application.DTOs.Base;
using ERP.Domain.Entities;

namespace ERP.Application.Interfaces.Repositories
{
    public interface ICostCenterRepository
    {
        Task<List<CostCenter>> GetAllAsync(long companyId);
        Task<PagedResult<CostCenter>> GetPagedAsync(long companyId, CostCenterFilterDTO filters);
        Task<CostCenter> GetOneByIdAsync(long costCenterId);
        Task<CostCenter> CreateAsync(CostCenter entity);
        Task<CostCenter> UpdateByIdAsync(long costCenterId, CostCenter entity);
        Task<bool> DeleteByIdAsync(long costCenterId);
    }
}
