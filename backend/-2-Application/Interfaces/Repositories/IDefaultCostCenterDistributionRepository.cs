using ERP.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERP.Application.Interfaces.Repositories
{
    public interface IDefaultCostCenterDistributionRepository
    {
        Task<List<DefaultCostCenterDistribution>> GetByCompanyIdAsync(long companyId);
        Task<DefaultCostCenterDistribution> CreateAsync(DefaultCostCenterDistribution entity);
        System.Threading.Tasks.Task DeleteByCompanyIdAsync(long companyId);
    }
}
