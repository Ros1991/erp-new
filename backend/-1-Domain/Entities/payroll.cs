//base: baseentity.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERP.Domain.Entities
{
	[Table("tb_payroll", Schema = "erp")]
	public class Payroll
	{
		[Key]
		//@AutoIncrement
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Column("payroll_id")]
		public long PayrollId { get; set; }

		[Column("company_id")]
		public long CompanyId { get; set; }

		[Column("payroll_period_start_date")]
		public DateTime PeriodStartDate { get; set; }

		[Column("payroll_period_end_date")]
		public DateTime PeriodEndDate { get; set; }

		[Column("payroll_total_gross_pay")]
		public long TotalGrossPay { get; set; }

		[Column("payroll_total_deductions")]
		public long TotalDeductions { get; set; }

		[Column("payroll_total_net_pay")]
		public long TotalNetPay { get; set; }

		[Column("payroll_is_closed")]
		public bool IsClosed { get; set; }

		[Column("payroll_closed_at")]
		public DateTime? ClosedAt { get; set; }

		[Column("payroll_closed_by")]
		public long? ClosedBy { get; set; }

		[Column("payroll_notes")]
		public string? Notes { get; set; }

		[Column("payroll_snapshot")]
		public string? Snapshot { get; set; }

		[Column("criado_por")]
		public long CriadoPor { get; set; }

		[Column("atualizado_por")]
		public long? AtualizadoPor { get; set; }

		[Column("criado_em")]
		public DateTime CriadoEm { get; set; }

		[Column("atualizado_em")]
		public DateTime? AtualizadoEm { get; set; }


		//Criando Relação com a tabelas
        public virtual Company Company { get; set; } = null!;
		public virtual User? ClosedByUser { get; set; }
		//Parent Relations
        public virtual ICollection<PayrollEmployee> PayrollEmployeeList { get; set; } = new List<PayrollEmployee>();
		// Construtor padrão para EF
		public Payroll() { }

		// Construtor com parâmetros
		public Payroll(
			long Param_CompanyId, 
			DateTime Param_PeriodStartDate, 
			DateTime Param_PeriodEndDate, 
			long Param_TotalGrossPay, 
			long Param_TotalDeductions, 
			long Param_TotalNetPay, 
			bool Param_IsClosed, 
			long Param_CriadoPor, 
			long? Param_AtualizadoPor, 
			DateTime Param_CriadoEm, 
			DateTime? Param_AtualizadoEm
		)
		{
			CompanyId = Param_CompanyId;
			PeriodStartDate = Param_PeriodStartDate;
			PeriodEndDate = Param_PeriodEndDate;
			TotalGrossPay = Param_TotalGrossPay;
			TotalDeductions = Param_TotalDeductions;
			TotalNetPay = Param_TotalNetPay;
			IsClosed = Param_IsClosed;
			CriadoPor = Param_CriadoPor;
			AtualizadoPor = Param_AtualizadoPor;
			CriadoEm = Param_CriadoEm;
			AtualizadoEm = Param_AtualizadoEm;
		}
	}
}
