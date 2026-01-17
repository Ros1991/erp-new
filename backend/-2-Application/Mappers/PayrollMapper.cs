using ERP.Application.DTOs;
using ERP.Domain.Entities;

namespace ERP.Application.Mappers
{
    public static class PayrollMapper
    {
        public static PayrollOutputDTO ToPayrollOutputDTO(Payroll entity, int employeeCount = 0, bool isLastPayroll = false)
        {
            if (entity == null) return null;

            return new PayrollOutputDTO
            {
                PayrollId = entity.PayrollId,
                CompanyId = entity.CompanyId,
                PeriodStartDate = entity.PeriodStartDate,
                PeriodEndDate = entity.PeriodEndDate,
                TotalGrossPay = entity.TotalGrossPay,
                TotalDeductions = entity.TotalDeductions,
                TotalNetPay = entity.TotalNetPay,
                TotalInss = entity.TotalInss,
                TotalFgts = entity.TotalFgts,
                ThirteenthPercentage = entity.ThirteenthPercentage,
                ThirteenthTaxOption = entity.ThirteenthTaxOption,
                IsClosed = entity.IsClosed,
                ClosedAt = entity.ClosedAt,
                ClosedBy = entity.ClosedBy,
                ClosedByName = null, // Pode ser preenchido no repository se necessário
                Notes = entity.Notes,
                Snapshot = entity.Snapshot,
                EmployeeCount = employeeCount,
                IsLastPayroll = isLastPayroll,
                CriadoPor = entity.CriadoPor,
                CriadoPorNome = "Sistema",
                AtualizadoPor = entity.AtualizadoPor,
                AtualizadoPorNome = null, // Será preenchido no repository se necessário
                CriadoEm = entity.CriadoEm,
                AtualizadoEm = entity.AtualizadoEm
            };
        }

        public static List<PayrollOutputDTO> ToPayrollOutputDTOList(IEnumerable<Payroll> entities)
        {
            return entities?.Select(e => ToPayrollOutputDTO(e)).ToList() ?? new List<PayrollOutputDTO>();
        }

        public static Payroll ToEntity(PayrollInputDTO dto, long companyId, long userId)
        {
            if (dto == null) return null;

            var now = DateTime.UtcNow;

            return new Payroll(
                companyId,
                DateTime.SpecifyKind(dto.PeriodStartDate, DateTimeKind.Utc),
                DateTime.SpecifyKind(dto.PeriodEndDate, DateTimeKind.Utc),
                0,             // TotalGrossPay (será calculado)
                0,             // TotalDeductions (será calculado)
                0,             // TotalNetPay (será calculado)
                false,         // IsClosed (sempre começa aberta)
                userId,        // CriadoPor
                null,          // AtualizadoPor
                now,           // CriadoEm
                null           // AtualizadoEm
            );
        }

        public static void UpdateEntity(Payroll entity, PayrollInputDTO dto, long userId)
        {
            if (entity == null || dto == null) return;
            
            entity.PeriodStartDate = DateTime.SpecifyKind(dto.PeriodStartDate, DateTimeKind.Utc);
            entity.PeriodEndDate = DateTime.SpecifyKind(dto.PeriodEndDate, DateTimeKind.Utc);
            entity.Notes = dto.Notes;
            entity.AtualizadoPor = userId;
            entity.AtualizadoEm = DateTime.UtcNow;
            // ✅ CriadoPor e CriadoEm NÃO são alterados
            // ✅ Totais são calculados automaticamente pelo service
            // ✅ IsClosed não é alterado aqui (método específico para fechar/reabrir)
        }
    }
}
