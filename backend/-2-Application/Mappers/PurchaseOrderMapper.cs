using ERP.Application.DTOs;
using ERP.Domain.Entities;

namespace ERP.Application.Mappers
{
    public static class PurchaseOrderMapper
    {
        public static PurchaseOrderOutputDTO ToPurchaseOrderOutputDTO(PurchaseOrder entity)
        {
            if (entity == null) return null;

            return new PurchaseOrderOutputDTO
            {
                PurchaseOrderId = entity.PurchaseOrderId,
                CompanyId = entity.CompanyId,
                UserIdRequester = entity.UserIdRequester,
                UserIdApprover = entity.UserIdApprover,
                Description = entity.Description,
                TotalAmount = entity.TotalAmount,
                Status = entity.Status,
                CriadoPor = entity.CriadoPor,
                AtualizadoPor = entity.AtualizadoPor,
                CriadoEm = entity.CriadoEm,
                AtualizadoEm = entity.AtualizadoEm
            };
        }

        public static List<PurchaseOrderOutputDTO> ToPurchaseOrderOutputDTOList(List<PurchaseOrder> entities)
        {
            if (entities == null) return new List<PurchaseOrderOutputDTO>();
            return entities.Select(ToPurchaseOrderOutputDTO).ToList();
        }

        public static PurchaseOrder ToEntity(PurchaseOrderInputDTO dto, long companyId, long userId)
        {
            if (dto == null) return null;

            var now = DateTime.UtcNow;

            return new PurchaseOrder(
                companyId,
                dto.UserIdRequester,
                dto.UserIdApprover,
                dto.Description,
                dto.TotalAmount,
                dto.Status,
                userId,        // CriadoPor
                null,          // AtualizadoPor
                now,           // CriadoEm
                null           // AtualizadoEm
            );
        }

        public static void UpdateEntity(PurchaseOrder entity, PurchaseOrderInputDTO dto, long userId)
        {
            if (entity == null || dto == null) return;
            
            entity.UserIdRequester = dto.UserIdRequester;
            entity.UserIdApprover = dto.UserIdApprover;
            entity.Description = dto.Description;
            entity.TotalAmount = dto.TotalAmount;
            entity.Status = dto.Status;
            entity.AtualizadoPor = userId;
            entity.AtualizadoEm = DateTime.UtcNow;
            // ✅ CriadoPor e CriadoEm NÃO são alterados
        }
    }
}
