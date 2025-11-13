using ERP.Application.DTOs;
using ERP.Domain.Entities;

namespace ERP.Application.Mappers
{
    public static class CompanySettingMapper
    {
        public static CompanySettingOutputDTO ToCompanySettingOutputDTO(CompanySetting entity)
        {
            if (entity == null) return null;

            return new CompanySettingOutputDTO
            {
                CompanySettingId = entity.CompanySettingId,
                CompanyId = entity.CompanyId,
                EmployeeIdGeneralManager = entity.EmployeeIdGeneralManager,
                TimeToleranceMinutes = entity.TimeToleranceMinutes,
                PayrollDay = entity.PayrollDay,
                PayrollClosingDay = entity.PayrollClosingDay,
                VacationDaysPerYear = entity.VacationDaysPerYear,
                WeeklyHoursDefault = entity.WeeklyHoursDefault,
                CriadoPor = entity.CriadoPor,
                AtualizadoPor = entity.AtualizadoPor,
                CriadoEm = entity.CriadoEm,
                AtualizadoEm = entity.AtualizadoEm
            };
        }

        public static List<CompanySettingOutputDTO> ToCompanySettingOutputDTOList(List<CompanySetting> entities)
        {
            if (entities == null) return new List<CompanySettingOutputDTO>();
            return entities.Select(ToCompanySettingOutputDTO).ToList();
        }

        public static CompanySetting ToEntity(CompanySettingInputDTO dto, long companyId, long userId)
        {
            if (dto == null) return null;

            var now = DateTime.UtcNow;

            return new CompanySetting(
                companyId,
                dto.EmployeeIdGeneralManager,
                dto.TimeToleranceMinutes,
                dto.PayrollDay,
                dto.PayrollClosingDay,
                dto.VacationDaysPerYear,
                dto.WeeklyHoursDefault,
                userId,
                null,
                now,
                null
            );
        }

        public static void UpdateEntity(CompanySetting entity, CompanySettingInputDTO dto, long userId)
        {
            if (entity == null || dto == null) return;
            
            entity.EmployeeIdGeneralManager = dto.EmployeeIdGeneralManager;
            entity.TimeToleranceMinutes = dto.TimeToleranceMinutes;
            entity.PayrollDay = dto.PayrollDay;
            entity.PayrollClosingDay = dto.PayrollClosingDay;
            entity.VacationDaysPerYear = dto.VacationDaysPerYear;
            entity.WeeklyHoursDefault = dto.WeeklyHoursDefault;
            entity.AtualizadoPor = userId;
            entity.AtualizadoEm = DateTime.UtcNow;
        }
    }
}
