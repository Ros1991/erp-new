using ERP.Application.DTOs;
using ERP.Domain.Entities;

namespace ERP.Application.Mappers
{
    public static class TaskEmployeeMapper
    {
        public static TaskEmployeeOutputDTO ToTaskEmployeeOutputDTO(TaskEmployee entity)
        {
            if (entity == null) return null;

            return new TaskEmployeeOutputDTO
            {
                TaskEmployeeId = entity.TaskEmployeeId,
                TaskId = entity.TaskId,
                EmployeeId = entity.EmployeeId,
                Status = entity.Status,
                EstimatedHours = entity.EstimatedHours,
                ActualHours = entity.ActualHours,
                StartDate = entity.StartDate,
                EndDate = entity.EndDate,
                CriadoPor = entity.CriadoPor,
                AtualizadoPor = entity.AtualizadoPor,
                CriadoEm = entity.CriadoEm,
                AtualizadoEm = entity.AtualizadoEm
            };
        }

        public static List<TaskEmployeeOutputDTO> ToTaskEmployeeOutputDTOList(List<TaskEmployee> entities)
        {
            if (entities == null) return new List<TaskEmployeeOutputDTO>();
            return entities.Select(ToTaskEmployeeOutputDTO).ToList();
        }

        public static TaskEmployee ToEntity(TaskEmployeeInputDTO dto, long userId)
        {
            if (dto == null) return null;

            var now = DateTime.UtcNow;

            return new TaskEmployee(
                dto.TaskId,
                dto.EmployeeId,
                dto.Status,
                dto.EstimatedHours,
                dto.ActualHours,
                dto.StartDate,
                dto.EndDate,
                userId,
                null,
                now,
                null
            );
        }

        public static void UpdateEntity(TaskEmployee entity, TaskEmployeeInputDTO dto, long userId)
        {
            if (entity == null || dto == null) return;
            
            entity.TaskId = dto.TaskId;
            entity.EmployeeId = dto.EmployeeId;
            entity.Status = dto.Status;
            entity.EstimatedHours = dto.EstimatedHours;
            entity.ActualHours = dto.ActualHours;
            entity.StartDate = dto.StartDate;
            entity.EndDate = dto.EndDate;
            entity.AtualizadoPor = userId;
            entity.AtualizadoEm = DateTime.UtcNow;
        }
    }
}
