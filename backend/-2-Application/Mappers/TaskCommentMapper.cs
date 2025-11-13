using ERP.Application.DTOs;
using ERP.Domain.Entities;

namespace ERP.Application.Mappers
{
    public static class TaskCommentMapper
    {
        public static TaskCommentOutputDTO ToTaskCommentOutputDTO(TaskComment entity)
        {
            if (entity == null) return null;

            return new TaskCommentOutputDTO
            {
                TaskCommentId = entity.TaskCommentId,
                TaskId = entity.TaskId,
                UserId = entity.UserId,
                Comment = entity.Comment,
                AttachmentUrl = entity.AttachmentUrl,
                CriadoPor = entity.CriadoPor,
                AtualizadoPor = entity.AtualizadoPor,
                CriadoEm = entity.CriadoEm,
                AtualizadoEm = entity.AtualizadoEm
            };
        }

        public static List<TaskCommentOutputDTO> ToTaskCommentOutputDTOList(List<TaskComment> entities)
        {
            if (entities == null) return new List<TaskCommentOutputDTO>();
            return entities.Select(ToTaskCommentOutputDTO).ToList();
        }

        public static TaskComment ToEntity(TaskCommentInputDTO dto, long userId)
        {
            if (dto == null) return null;

            var now = DateTime.UtcNow;

            return new TaskComment(
                dto.TaskId,
                dto.UserId,
                dto.Comment,
                dto.AttachmentUrl,
                userId,
                null,
                now,
                null
            );
        }

        public static void UpdateEntity(TaskComment entity, TaskCommentInputDTO dto, long userId)
        {
            if (entity == null || dto == null) return;
            
            entity.TaskId = dto.TaskId;
            entity.UserId = dto.UserId;
            entity.Comment = dto.Comment;
            entity.AttachmentUrl = dto.AttachmentUrl;
            entity.AtualizadoPor = userId;
            entity.AtualizadoEm = DateTime.UtcNow;
        }
    }
}
