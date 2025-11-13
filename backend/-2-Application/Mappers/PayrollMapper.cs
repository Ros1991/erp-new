using ERP.Application.DTOs;
using ERP.Domain.Entities;

namespace ERP.Application.Mappers
{
    public static class PayrollMapper
    {
        public static PayrollOutputDTO ToPayrollOutputDTO(Payroll entity)
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
                IsClosed = entity.IsClosed,
                CriadoPor = entity.CriadoPor,
                AtualizadoPor = entity.AtualizadoPor,
                CriadoEm = entity.CriadoEm,
                AtualizadoEm = entity.AtualizadoEm
            };
        }

        public static List<PayrollOutputDTO> ToPayrollOutputDTOList(List<Payroll> entities)
        {
            if (entities == null) return new List<PayrollOutputDTO>();
            return entities.Select(ToPayrollOutputDTO).ToList();
        }

        public static Payroll ToEntity(PayrollInputDTO dto, long companyId, long userId)
        {
            if (dto == null) return null;

            var now = DateTime.UtcNow;

            return new Payroll(
                companyId,
                dto.PeriodStartDate,
                dto.PeriodEndDate,
                dto.TotalGrossPay,
                dto.TotalDeductions,
                dto.TotalNetPay,
                dto.IsClosed,
                userId,
                null,
                now,
                null
            );
        }

        public static void UpdateEntity(Payroll entity, PayrollInputDTO dto, long userId)
        {
            if (entity == null || dto == null) return;
            
            entity.PeriodStartDate = dto.PeriodStartDate;
            entity.PeriodEndDate = dto.PeriodEndDate;
            entity.TotalGrossPay = dto.TotalGrossPay;
            entity.TotalDeductions = dto.TotalDeductions;
            entity.TotalNetPay = dto.TotalNetPay;
            entity.IsClosed = dto.IsClosed;
            entity.AtualizadoPor = userId;
            entity.AtualizadoEm = DateTime.UtcNow;
        }
    }
}
