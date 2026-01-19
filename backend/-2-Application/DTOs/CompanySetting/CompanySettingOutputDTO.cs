namespace ERP.Application.DTOs
{
    public class CompanySettingOutputDTO
    {
        public long CompanySettingId { get; set; }
        public long CompanyId { get; set; }
        public long? EmployeeIdGeneralManager { get; set; }
        public string? GeneralManagerName { get; set; }
        public long TimeToleranceMinutes { get; set; }
        public long PayrollDay { get; set; }
        public long PayrollClosingDay { get; set; }
        public long VacationDaysPerYear { get; set; }
        public long MinHoursForLunchBreak { get; set; }
        public long MaxOvertimeHoursPerMonth { get; set; }
        public bool AllowWeekendWork { get; set; }
        public long RequireJustificationAfterHours { get; set; }
        public long WeeklyHoursDefault { get; set; }
        public List<DefaultCostCenterDistributionDTO>? DefaultCostCenterDistributions { get; set; }
        public long CriadoPor { get; set; }
        public long? AtualizadoPor { get; set; }
        public DateTime CriadoEm { get; set; }
        public DateTime? AtualizadoEm { get; set; }
    }

    public class DefaultCostCenterDistributionDTO
    {
        public long DefaultCostCenterDistributionId { get; set; }
        public long CostCenterId { get; set; }
        public string? CostCenterName { get; set; }
        public decimal Percentage { get; set; }
    }
}
