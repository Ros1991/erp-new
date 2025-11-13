using ERP.Application.DTOs;
using ERP.Domain.Entities;

namespace ERP.Application.Mappers
{
    public static class TaskStatusHistoryMapper
    {
        public static TaskStatusHistoryOutputDTO ToTaskStatusHistoryOutputDTO(TaskStatusHistory entity)
        {
            if (entity == null) return null;

            return new TaskStatusHistoryOutputDTO
            {
                TaskStatusHistoryId = entity.TaskStatusHistoryId,
                TaskEmployeeId = entity.TaskEmployeeId,
                OldStatus = entity.OldStatus,
                NewStatus = entity.NewStatus,
                ChangeReason = entity.ChangeReason,
                CriadoPor = entity.CriadoPor,
                AtualizadoPor = entity.AtualizadoPor,
                CriadoEm = entity.CriadoEm,
                AtualizadoEm = entity.AtualizadoEm
            };
        }

        public static List<TaskStatusHistoryOutputDTO> ToTaskStatusHistoryOutputDTOList(List<TaskStatusHistory> entities)
        {
            if (entities == null) return new List<TaskStatusHistoryOutputDTO>();
            return entities.Select(ToTaskStatusHistoryOutputDTO).ToList();
        }

        public static TaskStatusHistory ToEntity(TaskStatusHistoryInputDTO dto, long userId)
        {
            if (dto == null) return null;

            var now = DateTime.UtcNow;

            return new TaskStatusHistory(
                dto.TaskEmployeeId,
                dto.OldStatus,
                dto.NewStatus,
                dto.ChangeReason,
                userId,
                null,
                now,
                null
            );
        }

        public static void UpdateEntity(TaskStatusHistory entity, TaskStatusHistoryInputDTO dto, long userId)
        {
            if (entity == null || dto == null) return;
            
            entity.TaskEmployeeId = dto.TaskEmployeeId;
            entity.OldStatus = dto.OldStatus;
            entity.NewStatus = dto.NewStatus;
            entity.ChangeReason = dto.ChangeReason;
            entity.AtualizadoPor = userId;
            entity.AtualizadoEm = DateTime.UtcNow;
        }
    }
}
