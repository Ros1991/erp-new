using ERP.Application.DTOs;
using Task = ERP.Domain.Entities.Task;

namespace ERP.Application.Mappers
{
    public static class TaskMapper
    {
        public static TaskOutputDTO ToTaskOutputDTO(Task entity)
        {
            if (entity == null) return null;

            return new TaskOutputDTO
            {
                TaskId = entity.TaskId,
                CompanyId = entity.CompanyId,
                TaskIdParent = entity.TaskIdParent,
                TaskIdBlocking = entity.TaskIdBlocking,
                Title = entity.Title,
                Description = entity.Description,
                Priority = entity.Priority,
                FrequencyDays = entity.FrequencyDays,
                AllowSunday = entity.AllowSunday,
                AllowMonday = entity.AllowMonday,
                AllowTuesday = entity.AllowTuesday,
                AllowWednesday = entity.AllowWednesday,
                AllowThursday = entity.AllowThursday,
                AllowFriday = entity.AllowFriday,
                AllowSaturday = entity.AllowSaturday,
                StartDate = entity.StartDate,
                EndDate = entity.EndDate,
                OverallStatus = entity.OverallStatus,
                CriadoPor = entity.CriadoPor,
                AtualizadoPor = entity.AtualizadoPor,
                CriadoEm = entity.CriadoEm,
                AtualizadoEm = entity.AtualizadoEm
            };
        }

        public static List<TaskOutputDTO> ToTaskOutputDTOList(List<Task> entities)
        {
            if (entities == null) return new List<TaskOutputDTO>();
            return entities.Select(ToTaskOutputDTO).ToList();
        }

        public static Task ToEntity(TaskInputDTO dto, long companyId, long userId)
        {
            if (dto == null) return null;

            var now = DateTime.UtcNow;

            return new Task(
                companyId,
                dto.TaskIdParent,
                dto.TaskIdBlocking,
                dto.Title,
                dto.Description,
                dto.Priority,
                dto.FrequencyDays,
                dto.AllowSunday,
                dto.AllowMonday,
                dto.AllowTuesday,
                dto.AllowWednesday,
                dto.AllowThursday,
                dto.AllowFriday,
                dto.AllowSaturday,
                dto.StartDate.HasValue ? DateTime.SpecifyKind(dto.StartDate.Value, DateTimeKind.Utc) : (DateTime?)null,
                dto.EndDate.HasValue ? DateTime.SpecifyKind(dto.EndDate.Value, DateTimeKind.Utc) : (DateTime?)null,
                dto.OverallStatus,
                userId,        // CriadoPor
                null,          // AtualizadoPor
                now,           // CriadoEm
                null           // AtualizadoEm
            );
        }

        public static void UpdateEntity(Task entity, TaskInputDTO dto, long userId)
        {
            if (entity == null || dto == null) return;
            
            entity.TaskIdParent = dto.TaskIdParent;
            entity.TaskIdBlocking = dto.TaskIdBlocking;
            entity.Title = dto.Title;
            entity.Description = dto.Description;
            entity.Priority = dto.Priority;
            entity.FrequencyDays = dto.FrequencyDays;
            entity.AllowSunday = dto.AllowSunday;
            entity.AllowMonday = dto.AllowMonday;
            entity.AllowTuesday = dto.AllowTuesday;
            entity.AllowWednesday = dto.AllowWednesday;
            entity.AllowThursday = dto.AllowThursday;
            entity.AllowFriday = dto.AllowFriday;
            entity.AllowSaturday = dto.AllowSaturday;
            entity.StartDate = dto.StartDate.HasValue ? DateTime.SpecifyKind(dto.StartDate.Value, DateTimeKind.Utc) : (DateTime?)null;
            entity.EndDate = dto.EndDate.HasValue ? DateTime.SpecifyKind(dto.EndDate.Value, DateTimeKind.Utc) : (DateTime?)null;
            entity.OverallStatus = dto.OverallStatus;
            entity.AtualizadoPor = userId;
            entity.AtualizadoEm = DateTime.UtcNow;
            // ✅ CriadoPor e CriadoEm NÃO são alterados
        }
    }
}
