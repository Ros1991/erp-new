using ERP.Application.DTOs;
using ERP.Domain.Entities;

namespace ERP.Application.Mappers
{
    public static class LoanAdvanceMapper
    {
        public static LoanAdvanceOutputDTO ToLoanAdvanceOutputDTO(LoanAdvance entity)
        {
            if (entity == null) return null;

            return new LoanAdvanceOutputDTO
            {
                LoanAdvanceId = entity.LoanAdvanceId,
                EmployeeId = entity.EmployeeId,
                EmployeeName = entity.Employee?.Nickname,
                Amount = entity.Amount,
                Installments = entity.Installments,
                DiscountSource = entity.DiscountSource,
                StartDate = entity.StartDate,
                IsApproved = entity.IsApproved,
                CriadoPor = entity.CriadoPor,
                AtualizadoPor = entity.AtualizadoPor,
                CriadoEm = entity.CriadoEm,
                AtualizadoEm = entity.AtualizadoEm
            };
        }

        public static List<LoanAdvanceOutputDTO> ToLoanAdvanceOutputDTOList(List<LoanAdvance> entities)
        {
            if (entities == null) return new List<LoanAdvanceOutputDTO>();
            return entities.Select(ToLoanAdvanceOutputDTO).ToList();
        }

        public static LoanAdvance ToEntity(LoanAdvanceInputDTO dto, long userId)
        {
            if (dto == null) return null;

            var now = DateTime.UtcNow;

            return new LoanAdvance(
                dto.EmployeeId,
                dto.Amount,
                dto.Installments,
                dto.DiscountSource,
                dto.StartDate,
                dto.IsApproved,
                userId,        // CriadoPor
                null,          // AtualizadoPor
                now,           // CriadoEm
                null           // AtualizadoEm
            );
        }

        public static void UpdateEntity(LoanAdvance entity, LoanAdvanceInputDTO dto, long userId)
        {
            if (entity == null || dto == null) return;
            
            entity.EmployeeId = dto.EmployeeId;
            entity.Amount = dto.Amount;
            entity.Installments = dto.Installments;
            entity.DiscountSource = dto.DiscountSource;
            entity.StartDate = dto.StartDate;
            entity.IsApproved = dto.IsApproved;
            entity.AtualizadoPor = userId;
            entity.AtualizadoEm = DateTime.UtcNow;
            // ✅ CriadoPor e CriadoEm NÃO são alterados
        }
    }
}
