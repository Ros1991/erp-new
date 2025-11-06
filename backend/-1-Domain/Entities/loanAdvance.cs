//base: baseentity.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CAU.Domain.Entities
{
	[Table("tb_loan_advance", Schema = "erp")]
	public class LoanAdvance
	{
		[Key]
		//@AutoIncrement
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Column("loan_advance_id")]
		public long LoanAdvanceId { get; set; }

		[Column("employee_id")]
		public long EmployeeId { get; set; }

		[Column("loan_advance_amount")]
		public long Amount { get; set; }

		[Column("loan_advance_installments")]
		public long Installments { get; set; }

		[Column("loan_advance_discount_source")]
		public string DiscountSource { get; set; }

		[Column("loan_advance_start_date")]
		public DateTime StartDate { get; set; }

		[Column("loan_advance_is_approved")]
		public bool IsApproved { get; set; }

		[Column("criado_por")]
		public long CriadoPor { get; set; }

		[Column("atualizado_por")]
		public long AtualizadoPor { get; set; }

		[Column("criado_em")]
		public DateTime CriadoEm { get; set; }

		[Column("atualizado_em")]
		public DateTime AtualizadoEm { get; set; }


		//Criando Relação com a tabelas
        public virtual Employee Employee { get; set; } = null!;
		// Construtor padrão para EF
		public LoanAdvance() { }

		// Construtor com parâmetros
		public LoanAdvance(
			long Param_EmployeeId, 
			long Param_Amount, 
			long Param_Installments, 
			string Param_DiscountSource, 
			DateTime Param_StartDate, 
			bool Param_IsApproved, 
			long Param_CriadoPor, 
			long Param_AtualizadoPor, 
			DateTime Param_CriadoEm, 
			DateTime Param_AtualizadoEm
		)
		{
			EmployeeId = Param_EmployeeId;
			Amount = Param_Amount;
			Installments = Param_Installments;
			DiscountSource = Param_DiscountSource;
			StartDate = Param_StartDate;
			IsApproved = Param_IsApproved;
			CriadoPor = Param_CriadoPor;
			AtualizadoPor = Param_AtualizadoPor;
			CriadoEm = Param_CriadoEm;
			AtualizadoEm = Param_AtualizadoEm;
		}
	}
}
