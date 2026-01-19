using ERP.Application.Interfaces.Repositories;
using ERP.Domain.Entities;
using ERP.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ERP.Infrastructure.Repositories
{
    public class CompanySettingRepository : ICompanySettingRepository
    {
        private readonly ErpContext _context;

        public CompanySettingRepository(ErpContext context)
        {
            _context = context;
        }

        public async Task<CompanySetting?> GetByCompanyIdAsync(long companyId)
        {
            return await _context.Set<CompanySetting>()
                .FirstOrDefaultAsync(cs => cs.CompanyId == companyId);
        }

        public async Task<CompanySetting> CreateAsync(CompanySetting entity)
        {
            await _context.Set<CompanySetting>().AddAsync(entity);
            return entity;
        }

        public async Task<CompanySetting> UpdateAsync(CompanySetting entity)
        {
            _context.Set<CompanySetting>().Update(entity);
            return entity;
        }
    }
}
