using ERP.Application.DTOs;
using ERP.Application.Interfaces;
using ERP.Application.Interfaces.Services;
using ERP.Application.Mappers;
using ERP.CrossCutting.Exceptions;

namespace ERP.Application.Services
{
    public class LocationService : ILocationService
    {
        private readonly IUnitOfWork _unitOfWork;

        public LocationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<LocationOutputDTO>> GetAllAsync(long companyId)
        {
            var entities = await _unitOfWork.LocationRepository.GetAllAsync(companyId);
            return LocationMapper.ToLocationOutputDTOList(entities);
        }

        public async Task<PagedResult<LocationOutputDTO>> GetPagedAsync(long companyId, LocationFilterDTO filters)
        {
            var pagedEntities = await _unitOfWork.LocationRepository.GetPagedAsync(companyId, filters);
            var dtoItems = LocationMapper.ToLocationOutputDTOList(pagedEntities.Items);
            
            return new PagedResult<LocationOutputDTO>(
                dtoItems,
                pagedEntities.Page,
                pagedEntities.PageSize,
                pagedEntities.Total
            );
        }

        public async Task<LocationOutputDTO> GetOneByIdAsync(long locationId)
        {
            var entity = await _unitOfWork.LocationRepository.GetOneByIdAsync(locationId);
            if (entity == null)
            {
                throw new EntityNotFoundException("Location", locationId);
            }
            return LocationMapper.ToLocationOutputDTO(entity);
        }

        public async Task<LocationOutputDTO> CreateAsync(LocationInputDTO dto, long companyId, long currentUserId)
        {
            if (dto == null)
            {
                throw new ValidationException(nameof(dto), "Dados são obrigatórios.");
            }

            var entity = LocationMapper.ToEntity(dto, companyId, currentUserId);
            
            var createdEntity = await _unitOfWork.LocationRepository.CreateAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return LocationMapper.ToLocationOutputDTO(createdEntity);
        }

        public async Task<LocationOutputDTO> UpdateByIdAsync(long locationId, LocationInputDTO dto, long currentUserId)
        {
            if (dto == null)
            {
                throw new ValidationException(nameof(dto), "Dados são obrigatórios.");
            }

            var existingEntity = await _unitOfWork.LocationRepository.GetOneByIdAsync(locationId);
            if (existingEntity == null)
            {
                throw new EntityNotFoundException("Location", locationId);
            }

            LocationMapper.UpdateEntity(existingEntity, dto, currentUserId);
            
            await _unitOfWork.SaveChangesAsync();
            return LocationMapper.ToLocationOutputDTO(existingEntity);
        }

        public async Task<bool> DeleteByIdAsync(long locationId)
        {
            var result = await _unitOfWork.LocationRepository.DeleteByIdAsync(locationId);
            await _unitOfWork.SaveChangesAsync();
            return result;
        }
    }
}
