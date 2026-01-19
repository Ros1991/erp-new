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
                RequesterName = entity.UserRequester?.Email,
                UserIdApprover = entity.UserIdApprover,
                ApproverName = entity.UserAprrover?.Email,
                Description = entity.Description,
                TotalAmount = entity.TotalAmount,
                Status = entity.Status,
                ProcessedMessage = entity.ProcessedMessage,
                ProcessedAt = entity.ProcessedAt,
                AccountId = entity.AccountId,
                AccountName = entity.Account?.Name,
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

            return new PurchaseOrder
            {
                CompanyId = companyId,
                UserIdRequester = userId,
                Description = dto.Description,
                TotalAmount = 0,
                Status = "Pendente",
                CriadoPor = userId,
                CriadoEm = now
            };
        }

        public static void UpdateEntity(PurchaseOrder entity, PurchaseOrderInputDTO dto, long userId)
        {
            if (entity == null || dto == null) return;
            
            entity.Description = dto.Description;
            entity.AtualizadoPor = userId;
            entity.AtualizadoEm = DateTime.UtcNow;
        }
    }
}
