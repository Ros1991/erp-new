using ERP.Application.DTOs;
using ERP.Domain.Entities;

namespace ERP.Application.Mappers
{
    public static class TimeEntryMapper
    {
        public static TimeEntryOutputDTO ToTimeEntryOutputDTO(TimeEntry entity)
        {
            if (entity == null) return null;

            return new TimeEntryOutputDTO
            {
                TimeEntryId = entity.TimeEntryId,
                EmployeeId = entity.EmployeeId,
                Type = entity.Type,
                Timestamp = entity.Timestamp,
                Latitude = entity.Latitude,
                Longitude = entity.Longitude,
                LocationId = entity.LocationId,
                CriadoPor = entity.CriadoPor,
                AtualizadoPor = entity.AtualizadoPor,
                CriadoEm = entity.CriadoEm,
                AtualizadoEm = entity.AtualizadoEm
            };
        }

        public static List<TimeEntryOutputDTO> ToTimeEntryOutputDTOList(List<TimeEntry> entities)
        {
            if (entities == null) return new List<TimeEntryOutputDTO>();
            return entities.Select(ToTimeEntryOutputDTO).ToList();
        }

        public static TimeEntry ToEntity(TimeEntryInputDTO dto, long userId)
        {
            if (dto == null) return null;

            var now = DateTime.UtcNow;

            return new TimeEntry(
                dto.EmployeeId,
                dto.Type,
                dto.Timestamp,
                dto.Latitude,
                dto.Longitude,
                dto.LocationId,
                userId,
                null,
                now,
                null
            );
        }

        public static void UpdateEntity(TimeEntry entity, TimeEntryInputDTO dto, long userId)
        {
            if (entity == null || dto == null) return;
            
            entity.EmployeeId = dto.EmployeeId;
            entity.Type = dto.Type;
            entity.Timestamp = dto.Timestamp;
            entity.Latitude = dto.Latitude;
            entity.Longitude = dto.Longitude;
            entity.LocationId = dto.LocationId;
            entity.AtualizadoPor = userId;
            entity.AtualizadoEm = DateTime.UtcNow;
        }
    }
}
