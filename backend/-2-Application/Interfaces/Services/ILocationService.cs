using ERP.Application.DTOs;

namespace ERP.Application.Interfaces.Services
{
    public interface ILocationService
    {
        Task<List<LocationOutputDTO>> GetAllAsync(long companyId);
        Task<PagedResult<LocationOutputDTO>> GetPagedAsync(long companyId, LocationFilterDTO filters);
        Task<LocationOutputDTO> GetOneByIdAsync(long locationId);
        Task<LocationOutputDTO> CreateAsync(LocationInputDTO dto, long companyId, long currentUserId);
        Task<LocationOutputDTO> UpdateByIdAsync(long locationId, LocationInputDTO dto, long currentUserId);
        Task<bool> DeleteByIdAsync(long locationId);
    }
}
