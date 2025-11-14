using ERP.Application.DTOs;

namespace ERP.Application.Interfaces.Services
{
    public interface ICostCenterService
    {
        Task<List<CostCenterOutputDTO>> GetAllAsync(long companyId);
        Task<PagedResult<CostCenterOutputDTO>> GetPagedAsync(long companyId, CostCenterFilterDTO filters);
        Task<CostCenterOutputDTO> GetOneByIdAsync(long costCenterId);
        Task<CostCenterOutputDTO> CreateAsync(CostCenterInputDTO dto, long companyId, long currentUserId);
        Task<CostCenterOutputDTO> UpdateByIdAsync(long costCenterId, CostCenterInputDTO dto, long currentUserId);
        Task<bool> DeleteByIdAsync(long costCenterId);
    }
}
