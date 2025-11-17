//base: baseentity.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERP.Domain.Entities
{
	[Table("tb_payroll_employee", Schema = "erp")]
	public class PayrollEmployee
	{
		[Key]
		//@AutoIncrement
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Column("payroll_employee_id")]
		public long PayrollEmployeeId { get; set; }

		[Column("payroll_id")]
		public long PayrollId { get; set; }

		[Column("employee_id")]
		public long EmployeeId { get; set; }

		[Column("contract_id")]
		public long? ContractId { get; set; }

		[Column("payroll_employee_base_salary")]
		public decimal BaseSalary { get; set; }

		[Column("payroll_employee_is_on_vacation")]
		public bool IsOnVacation { get; set; }

		[Column("payroll_employee_vacation_days")]
		public long? VacationDays { get; set; }

		[Column("payroll_employee_vacation_advance_amount")]
		public long? VacationAdvanceAmount { get; set; }

		[Column("payroll_employee_vacation_advance_paid")]
		public bool VacationAdvancePaid { get; set; }

		[Column("payroll_employee_vacation_start_date")]
		public DateTime? VacationStartDate { get; set; }

		[Column("payroll_employee_vacation_end_date")]
		public DateTime? VacationEndDate { get; set; }

		[Column("payroll_employee_vacation_notes")]
		public string? VacationNotes { get; set; }

		[Column("payroll_employee_has_13th")]
		public bool Has13th { get; set; }

		[Column("payroll_employee_13th_type")]
		public string? ThirteenthType { get; set; }

		[Column("payroll_employee_13th_amount")]
		public decimal? ThirteenthAmount { get; set; }

		[Column("payroll_employee_13th_tax_option")]
		public string? ThirteenthTaxOption { get; set; }

		[Column("payroll_employee_notes")]
		public string? Notes { get; set; }

		[Column("payroll_employee_total_gross_pay")]
		public long TotalGrossPay { get; set; }

		[Column("payroll_employee_total_deductions")]
		public long TotalDeductions { get; set; }

		[Column("payroll_employee_total_net_pay")]
		public long TotalNetPay { get; set; }

		[Column("criado_por")]
		public long CriadoPor { get; set; }

		[Column("atualizado_por")]
		public long? AtualizadoPor { get; set; }

		[Column("criado_em")]
		public DateTime CriadoEm { get; set; }

		[Column("atualizado_em")]
		public DateTime? AtualizadoEm { get; set; }


		//Criando Relação com a tabelas
        public virtual Employee Employee { get; set; } = null!;
		public virtual Contract? Contract { get; set; }
		//Criando Relação com a tabelas
        public virtual Payroll Payroll { get; set; } = null!;
		//Parent Relations
        public virtual ICollection<PayrollItem> PayrollItemList { get; set; } = new List<PayrollItem>();
		// Construtor padrão para EF
		public PayrollEmployee() { }

		// Construtor com parâmetros
		public PayrollEmployee(
			long Param_PayrollId, 
			long Param_EmployeeId, 
			bool Param_IsOnVacation, 
			long? Param_VacationDays, 
			long? Param_VacationAdvanceAmount, 
			long Param_TotalGrossPay, 
			long Param_TotalDeductions, 
			long Param_TotalNetPay, 
			long Param_CriadoPor, 
			long? Param_AtualizadoPor, 
			DateTime Param_CriadoEm, 
			DateTime? Param_AtualizadoEm
		)
		{
			PayrollId = Param_PayrollId;
			EmployeeId = Param_EmployeeId;
			IsOnVacation = Param_IsOnVacation;
			VacationDays = Param_VacationDays;
			VacationAdvanceAmount = Param_VacationAdvanceAmount;
			TotalGrossPay = Param_TotalGrossPay;
			TotalDeductions = Param_TotalDeductions;
			TotalNetPay = Param_TotalNetPay;
			CriadoPor = Param_CriadoPor;
			AtualizadoPor = Param_AtualizadoPor;
			CriadoEm = Param_CriadoEm;
			AtualizadoEm = Param_AtualizadoEm;
		}
	}
}
