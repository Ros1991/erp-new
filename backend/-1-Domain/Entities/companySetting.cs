//base: baseentity.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CAU.Domain.Entities
{
	[Table("tb_company_setting", Schema = "erp")]
	public class CompanySetting
	{
		[Key]
		//@AutoIncrement
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Column("company_setting_id")]
		public long CompanySettingId { get; set; }

		[Column("company_id")]
		public long CompanyId { get; set; }

		[Column("employee_id_general_manager")]
		public long EmployeeIdGeneralManager { get; set; }

		[Column("company_setting_time_tolerance_minutes")]
		public long TimeToleranceMinutes { get; set; }

		[Column("company_setting_payroll_day")]
		public long PayrollDay { get; set; }

		[Column("company_setting_payroll_closing_day")]
		public long PayrollClosingDay { get; set; }

		[Column("company_setting_vacation_days_per_year")]
		public long VacationDaysPerYear { get; set; }

		[Column("company_setting_min_hours_for_lunch_break")]
		public long MinHoursForLunchBreak { get; set; }

		[Column("company_setting_max_overtime_hours_per_month")]
		public long MaxOvertimeHoursPerMonth { get; set; }

		[Column("company_setting_allow_weekend_work")]
		public bool AllowWeekendWork { get; set; }

		[Column("company_setting_require_justification_after_hours")]
		public long RequireJustificationAfterHours { get; set; }

		[Column("company_setting_weekly_hours_default")]
		public long WeeklyHoursDefault { get; set; }

		[Column("criado_por")]
		public long CriadoPor { get; set; }

		[Column("atualizado_por")]
		public long AtualizadoPor { get; set; }

		[Column("criado_em")]
		public DateTime CriadoEm { get; set; }

		[Column("atualizado_em")]
		public DateTime AtualizadoEm { get; set; }


		//Criando Relação com a tabelas
        public virtual Company Company { get; set; } = null!;
		// Construtor padrão para EF
		public CompanySetting() { }

		// Construtor com parâmetros
		public CompanySetting(
			long Param_CompanyId, 
			long Param_EmployeeIdGeneralManager, 
			long Param_TimeToleranceMinutes, 
			long Param_PayrollDay, 
			long Param_PayrollClosingDay, 
			long Param_VacationDaysPerYear, 
			long Param_MinHoursForLunchBreak, 
			long Param_MaxOvertimeHoursPerMonth, 
			bool Param_AllowWeekendWork, 
			long Param_RequireJustificationAfterHours, 
			long Param_WeeklyHoursDefault, 
			long Param_CriadoPor, 
			long Param_AtualizadoPor, 
			DateTime Param_CriadoEm, 
			DateTime Param_AtualizadoEm
		)
		{
			CompanyId = Param_CompanyId;
			EmployeeIdGeneralManager = Param_EmployeeIdGeneralManager;
			TimeToleranceMinutes = Param_TimeToleranceMinutes;
			PayrollDay = Param_PayrollDay;
			PayrollClosingDay = Param_PayrollClosingDay;
			VacationDaysPerYear = Param_VacationDaysPerYear;
			MinHoursForLunchBreak = Param_MinHoursForLunchBreak;
			MaxOvertimeHoursPerMonth = Param_MaxOvertimeHoursPerMonth;
			AllowWeekendWork = Param_AllowWeekendWork;
			RequireJustificationAfterHours = Param_RequireJustificationAfterHours;
			WeeklyHoursDefault = Param_WeeklyHoursDefault;
			CriadoPor = Param_CriadoPor;
			AtualizadoPor = Param_AtualizadoPor;
			CriadoEm = Param_CriadoEm;
			AtualizadoEm = Param_AtualizadoEm;
		}
	}
}
