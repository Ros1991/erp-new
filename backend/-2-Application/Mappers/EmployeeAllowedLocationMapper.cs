using ERP.Application.DTOs;
using ERP.Domain.Entities;

namespace ERP.Application.Mappers
{
    public static class EmployeeAllowedLocationMapper
    {
        public static EmployeeAllowedLocationOutputDTO ToEmployeeAllowedLocationOutputDTO(EmployeeAllowedLocation entity)
        {
            if (entity == null) return null;

            return new EmployeeAllowedLocationOutputDTO
            {
                EmployeeAllowedLocationId = entity.EmployeeAllowedLocationId,
                EmployeeId = entity.EmployeeId,
                LocationId = entity.LocationId,
                CriadoPor = entity.CriadoPor,
                AtualizadoPor = entity.AtualizadoPor,
                CriadoEm = entity.CriadoEm,
                AtualizadoEm = entity.AtualizadoEm
            };
        }

        public static List<EmployeeAllowedLocationOutputDTO> ToEmployeeAllowedLocationOutputDTOList(List<EmployeeAllowedLocation> entities)
        {
            if (entities == null) return new List<EmployeeAllowedLocationOutputDTO>();
            return entities.Select(ToEmployeeAllowedLocationOutputDTO).ToList();
        }

        public static EmployeeAllowedLocation ToEntity(EmployeeAllowedLocationInputDTO dto, long userId)
        {
            if (dto == null) return null;

            var now = DateTime.UtcNow;

            return new EmployeeAllowedLocation(
                dto.EmployeeId,
                dto.LocationId,
                userId,
                null,
                now,
                null
            );
        }

        public static void UpdateEntity(EmployeeAllowedLocation entity, EmployeeAllowedLocationInputDTO dto, long userId)
        {
            if (entity == null || dto == null) return;
            
            entity.EmployeeId = dto.EmployeeId;
            entity.LocationId = dto.LocationId;
            entity.AtualizadoPor = userId;
            entity.AtualizadoEm = DateTime.UtcNow;
        }
    }
}
