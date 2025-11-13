using ERP.Application.DTOs;
using ERP.Application.DTOs.Base;
using ERP.Domain.Entities;

namespace ERP.Application.Interfaces.Repositories
{
    public interface ILocationRepository
    {
        Task<List<Location>> GetAllAsync(long companyId);
        Task<PagedResult<Location>> GetPagedAsync(long companyId, LocationFilterDTO filters);
        Task<Location> GetOneByIdAsync(long locationId);
        Task<Location> CreateAsync(Location entity);
        Task<Location> UpdateByIdAsync(long locationId, Location entity);
        Task<bool> DeleteByIdAsync(long locationId);
    }
}
