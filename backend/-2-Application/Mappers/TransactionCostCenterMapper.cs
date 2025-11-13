using ERP.Application.DTOs;
using ERP.Domain.Entities;

namespace ERP.Application.Mappers
{
    public static class TransactionCostCenterMapper
    {
        public static TransactionCostCenterOutputDTO ToTransactionCostCenterOutputDTO(TransactionCostCenter entity)
        {
            if (entity == null) return null;

            return new TransactionCostCenterOutputDTO
            {
                TransactionCostCenterId = entity.TransactionCostCenterId,
                FinancialTransactionId = entity.FinancialTransactionId,
                CostCenterId = entity.CostCenterId,
                Amount = entity.Amount,
                Percentage = entity.Percentage,
                CriadoPor = entity.CriadoPor,
                AtualizadoPor = entity.AtualizadoPor,
                CriadoEm = entity.CriadoEm,
                AtualizadoEm = entity.AtualizadoEm
            };
        }

        public static List<TransactionCostCenterOutputDTO> ToTransactionCostCenterOutputDTOList(List<TransactionCostCenter> entities)
        {
            if (entities == null) return new List<TransactionCostCenterOutputDTO>();
            return entities.Select(ToTransactionCostCenterOutputDTO).ToList();
        }

        public static TransactionCostCenter ToEntity(TransactionCostCenterInputDTO dto, long userId)
        {
            if (dto == null) return null;

            var now = DateTime.UtcNow;

            return new TransactionCostCenter(
                dto.FinancialTransactionId,
                dto.CostCenterId,
                dto.Amount,
                dto.Percentage,
                userId,
                null,
                now,
                null
            );
        }

        public static void UpdateEntity(TransactionCostCenter entity, TransactionCostCenterInputDTO dto, long userId)
        {
            if (entity == null || dto == null) return;
            
            entity.FinancialTransactionId = dto.FinancialTransactionId;
            entity.CostCenterId = dto.CostCenterId;
            entity.Amount = dto.Amount;
            entity.Percentage = dto.Percentage;
            entity.AtualizadoPor = userId;
            entity.AtualizadoEm = DateTime.UtcNow;
        }
    }
}
