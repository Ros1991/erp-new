using ERP.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERP.Application.Interfaces.Services
{
    public interface ICompanySettingService
    {
        Task<CompanySettingOutputDTO> GetByCompanyIdAsync(long companyId);
        Task<CompanySettingOutputDTO> SaveAsync(CompanySettingInputDTO dto, long companyId, long currentUserId);
        Task<List<DefaultCostCenterDistributionDTO>> GetDefaultDistributionsAsync(long companyId);
    }
}
