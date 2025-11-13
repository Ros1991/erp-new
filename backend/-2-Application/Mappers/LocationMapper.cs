using ERP.Application.DTOs;
using ERP.Domain.Entities;

namespace ERP.Application.Mappers
{
    public static class LocationMapper
    {
        public static LocationOutputDTO ToLocationOutputDTO(Location entity)
        {
            if (entity == null) return null;

            return new LocationOutputDTO
            {
                LocationId = entity.LocationId,
                CompanyId = entity.CompanyId,
                Name = entity.Name,
                Address = entity.Address,
                Latitude = entity.Latitude,
                Longitude = entity.Longitude,
                RadiusMeters = entity.RadiusMeters,
                IsActive = entity.IsActive,
                CriadoPor = entity.CriadoPor,
                AtualizadoPor = entity.AtualizadoPor,
                CriadoEm = entity.CriadoEm,
                AtualizadoEm = entity.AtualizadoEm
            };
        }

        public static List<LocationOutputDTO> ToLocationOutputDTOList(List<Location> entities)
        {
            if (entities == null) return new List<LocationOutputDTO>();
            return entities.Select(ToLocationOutputDTO).ToList();
        }

        public static Location ToEntity(LocationInputDTO dto, long companyId, long userId)
        {
            if (dto == null) return null;

            var now = DateTime.UtcNow;

            return new Location(
                companyId,
                dto.Name,
                dto.Address,
                dto.Latitude,
                dto.Longitude,
                dto.RadiusMeters,
                dto.IsActive,
                userId,        // CriadoPor
                null,          // AtualizadoPor
                now,           // CriadoEm
                null           // AtualizadoEm
            );
        }

        public static void UpdateEntity(Location entity, LocationInputDTO dto, long userId)
        {
            if (entity == null || dto == null) return;
            
            entity.Name = dto.Name;
            entity.Address = dto.Address;
            entity.Latitude = dto.Latitude;
            entity.Longitude = dto.Longitude;
            entity.RadiusMeters = dto.RadiusMeters;
            entity.IsActive = dto.IsActive;
            entity.AtualizadoPor = userId;
            entity.AtualizadoEm = DateTime.UtcNow;
            // ✅ CriadoPor e CriadoEm NÃO são alterados
        }
    }
}
