using ERP.Application.Interfaces.Repositories;
using ERP.Domain.Entities;
using ERP.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ERP.Infrastructure.Repositories
{
    public class DefaultCostCenterDistributionRepository : IDefaultCostCenterDistributionRepository
    {
        private readonly ErpContext _context;

        public DefaultCostCenterDistributionRepository(ErpContext context)
        {
            _context = context;
        }

        public async Task<List<DefaultCostCenterDistribution>> GetByCompanyIdAsync(long companyId)
        {
            return await _context.Set<DefaultCostCenterDistribution>()
                .Include(d => d.CostCenter)
                .Where(d => d.CompanyId == companyId)
                .ToListAsync();
        }

        public async Task<DefaultCostCenterDistribution> CreateAsync(DefaultCostCenterDistribution entity)
        {
            await _context.Set<DefaultCostCenterDistribution>().AddAsync(entity);
            return entity;
        }

        public async System.Threading.Tasks.Task DeleteByCompanyIdAsync(long companyId)
        {
            var items = await _context.Set<DefaultCostCenterDistribution>()
                .Where(d => d.CompanyId == companyId)
                .ToListAsync();
            
            _context.Set<DefaultCostCenterDistribution>().RemoveRange(items);
        }
    }
}
