using ERP.Application.DTOs;
using ERP.Domain.Entities;

namespace ERP.Application.Mappers
{
    public static class PayrollItemMapper
    {
        public static PayrollItemOutputDTO ToPayrollItemOutputDTO(PayrollItem entity)
        {
            if (entity == null) return null;

            return new PayrollItemOutputDTO
            {
                PayrollItemId = entity.PayrollItemId,
                PayrollEmployeeId = entity.PayrollEmployeeId,
                Description = entity.Description,
                Type = entity.Type,
                Category = entity.Category,
                Amount = entity.Amount,
                ReferenceId = entity.ReferenceId,
                CalculationBasis = entity.CalculationBasis,
                CalculationDetails = entity.CalculationDetails,
                CriadoPor = entity.CriadoPor,
                AtualizadoPor = entity.AtualizadoPor,
                CriadoEm = entity.CriadoEm,
                AtualizadoEm = entity.AtualizadoEm
            };
        }

        public static List<PayrollItemOutputDTO> ToPayrollItemOutputDTOList(List<PayrollItem> entities)
        {
            if (entities == null) return new List<PayrollItemOutputDTO>();
            return entities.Select(ToPayrollItemOutputDTO).ToList();
        }

        public static PayrollItem ToEntity(PayrollItemInputDTO dto, long userId)
        {
            if (dto == null) return null;

            var now = DateTime.UtcNow;

            return new PayrollItem(
                dto.PayrollEmployeeId,
                dto.Description,
                dto.Type,
                dto.Category,
                dto.Amount,
                dto.ReferenceId,
                dto.CalculationBasis,
                dto.CalculationDetails,
                userId,
                null,
                now,
                null
            );
        }

        public static void UpdateEntity(PayrollItem entity, PayrollItemInputDTO dto, long userId)
        {
            if (entity == null || dto == null) return;
            
            entity.PayrollEmployeeId = dto.PayrollEmployeeId;
            entity.Description = dto.Description;
            entity.Type = dto.Type;
            entity.Category = dto.Category;
            entity.Amount = dto.Amount;
            entity.ReferenceId = dto.ReferenceId;
            entity.CalculationBasis = dto.CalculationBasis;
            entity.CalculationDetails = dto.CalculationDetails;
            entity.AtualizadoPor = userId;
            entity.AtualizadoEm = DateTime.UtcNow;
        }
    }
}
