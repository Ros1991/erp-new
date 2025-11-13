using ERP.Application.DTOs;
using ERP.Domain.Entities;

namespace ERP.Application.Mappers
{
    public static class PayrollEmployeeMapper
    {
        public static PayrollEmployeeOutputDTO ToPayrollEmployeeOutputDTO(PayrollEmployee entity)
        {
            if (entity == null) return null;

            return new PayrollEmployeeOutputDTO
            {
                PayrollEmployeeId = entity.PayrollEmployeeId,
                PayrollId = entity.PayrollId,
                EmployeeId = entity.EmployeeId,
                IsOnVacation = entity.IsOnVacation,
                VacationDays = entity.VacationDays,
                VacationAdvanceAmount = entity.VacationAdvanceAmount,
                TotalGrossPay = entity.TotalGrossPay,
                TotalDeductions = entity.TotalDeductions,
                TotalNetPay = entity.TotalNetPay,
                CriadoPor = entity.CriadoPor,
                AtualizadoPor = entity.AtualizadoPor,
                CriadoEm = entity.CriadoEm,
                AtualizadoEm = entity.AtualizadoEm
            };
        }

        public static List<PayrollEmployeeOutputDTO> ToPayrollEmployeeOutputDTOList(List<PayrollEmployee> entities)
        {
            if (entities == null) return new List<PayrollEmployeeOutputDTO>();
            return entities.Select(ToPayrollEmployeeOutputDTO).ToList();
        }

        public static PayrollEmployee ToEntity(PayrollEmployeeInputDTO dto, long userId)
        {
            if (dto == null) return null;

            var now = DateTime.UtcNow;

            return new PayrollEmployee(
                dto.PayrollId,
                dto.EmployeeId,
                dto.IsOnVacation,
                dto.VacationDays,
                dto.VacationAdvanceAmount,
                dto.TotalGrossPay,
                dto.TotalDeductions,
                dto.TotalNetPay,
                userId,
                null,
                now,
                null
            );
        }

        public static void UpdateEntity(PayrollEmployee entity, PayrollEmployeeInputDTO dto, long userId)
        {
            if (entity == null || dto == null) return;
            
            entity.PayrollId = dto.PayrollId;
            entity.EmployeeId = dto.EmployeeId;
            entity.IsOnVacation = dto.IsOnVacation;
            entity.VacationDays = dto.VacationDays;
            entity.VacationAdvanceAmount = dto.VacationAdvanceAmount;
            entity.TotalGrossPay = dto.TotalGrossPay;
            entity.TotalDeductions = dto.TotalDeductions;
            entity.TotalNetPay = dto.TotalNetPay;
            entity.AtualizadoPor = userId;
            entity.AtualizadoEm = DateTime.UtcNow;
        }
    }
}
