using ERP.Domain.Entities;
using System.Threading.Tasks;

namespace ERP.Application.Interfaces.Repositories
{
    public interface ICompanySettingRepository
    {
        Task<CompanySetting?> GetByCompanyIdAsync(long companyId);
        Task<CompanySetting> CreateAsync(CompanySetting entity);
        Task<CompanySetting> UpdateAsync(CompanySetting entity);
    }
}
