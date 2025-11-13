using ERP.Application.DTOs;
using ERP.Domain.Entities;

namespace ERP.Application.Mappers
{
    public static class JustificationMapper
    {
        public static JustificationOutputDTO ToJustificationOutputDTO(Justification entity)
        {
            if (entity == null) return null;

            return new JustificationOutputDTO
            {
                JustificationId = entity.JustificationId,
                EmployeeId = entity.EmployeeId,
                ReferenceDate = entity.ReferenceDate,
                Reason = entity.Reason,
                AttachmentUrl = entity.AttachmentUrl,
                HoursGranted = entity.HoursGranted,
                UserIdApprover = entity.UserIdApprover,
                Status = entity.Status,
                CriadoPor = entity.CriadoPor,
                AtualizadoPor = entity.AtualizadoPor,
                CriadoEm = entity.CriadoEm,
                AtualizadoEm = entity.AtualizadoEm
            };
        }

        public static List<JustificationOutputDTO> ToJustificationOutputDTOList(List<Justification> entities)
        {
            if (entities == null) return new List<JustificationOutputDTO>();
            return entities.Select(ToJustificationOutputDTO).ToList();
        }

        public static Justification ToEntity(JustificationInputDTO dto, long userId)
        {
            if (dto == null) return null;

            var now = DateTime.UtcNow;

            return new Justification(
                dto.EmployeeId,
                dto.ReferenceDate,
                dto.Reason,
                dto.AttachmentUrl,
                dto.HoursGranted,
                dto.UserIdApprover,
                dto.Status,
                userId,
                null,
                now,
                null
            );
        }

        public static void UpdateEntity(Justification entity, JustificationInputDTO dto, long userId)
        {
            if (entity == null || dto == null) return;
            
            entity.EmployeeId = dto.EmployeeId;
            entity.ReferenceDate = dto.ReferenceDate;
            entity.Reason = dto.Reason;
            entity.AttachmentUrl = dto.AttachmentUrl;
            entity.HoursGranted = dto.HoursGranted;
            entity.UserIdApprover = dto.UserIdApprover;
            entity.Status = dto.Status;
            entity.AtualizadoPor = userId;
            entity.AtualizadoEm = DateTime.UtcNow;
        }
    }
}
