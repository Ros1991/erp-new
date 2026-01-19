using System.ComponentModel.DataAnnotations;

namespace ERP.Application.DTOs
{
    public class CompanySettingInputDTO
    {
        public long? EmployeeIdGeneralManager { get; set; }

        [Required(ErrorMessage = "Tolerância de tempo em minutos é obrigatória")]
        public long TimeToleranceMinutes { get; set; }

        [Required(ErrorMessage = "Dia do pagamento é obrigatório")]
        public long PayrollDay { get; set; }

        [Required(ErrorMessage = "Dia de fechamento da folha é obrigatório")]
        public long PayrollClosingDay { get; set; }

        [Required(ErrorMessage = "Dias de férias por ano é obrigatório")]
        public long VacationDaysPerYear { get; set; }

        [Required(ErrorMessage = "Horas mínimas para intervalo de almoço é obrigatório")]
        public long MinHoursForLunchBreak { get; set; }

        [Required(ErrorMessage = "Máximo de horas extras por mês é obrigatório")]
        public long MaxOvertimeHoursPerMonth { get; set; }

        [Required(ErrorMessage = "Permitir trabalho aos finais de semana é obrigatório")]
        public bool AllowWeekendWork { get; set; }

        [Required(ErrorMessage = "Horas para exigir justificativa é obrigatório")]
        public long RequireJustificationAfterHours { get; set; }

        [Required(ErrorMessage = "Horas semanais padrão é obrigatório")]
        public long WeeklyHoursDefault { get; set; }

        // Rateio padrão de centros de custo
        public List<DefaultCostCenterDistributionInputDTO>? DefaultCostCenterDistributions { get; set; }
    }

    public class DefaultCostCenterDistributionInputDTO
    {
        public long CostCenterId { get; set; }
        public decimal Percentage { get; set; }
    }
}
