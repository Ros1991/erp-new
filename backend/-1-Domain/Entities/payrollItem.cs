//base: baseentity.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERP.Domain.Entities
{
	[Table("tb_payroll_item", Schema = "erp")]
	public class PayrollItem
	{
		[Key]
		//@AutoIncrement
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Column("payroll_item_id")]
		public long PayrollItemId { get; set; }

		[Column("payroll_employee_id")]
		public long PayrollEmployeeId { get; set; }

		[Column("payroll_item_description")]
		public string Description { get; set; }

		[Column("payroll_item_type")]
		public string Type { get; set; }

		[Column("payroll_item_category")]
		public string Category { get; set; }

		[Column("payroll_item_amount")]
		public long Amount { get; set; }

		[Column("payroll_item_reference_id")]
		public long? ReferenceId { get; set; }

		[Column("payroll_item_calculation_basis")]
		public long? CalculationBasis { get; set; }

		[Column("payroll_item_calculation_details", TypeName = "jsonb")]
		public string? CalculationDetails { get; set; }

		[Column("payroll_item_is_manual")]
		public bool IsManual { get; set; }

		[Column("payroll_item_is_active")]
		public bool IsActive { get; set; } = true;

		[Column("payroll_item_source_type")]
		public string? SourceType { get; set; }

		[Column("payroll_item_installment_number")]
		public int? InstallmentNumber { get; set; }

		[Column("payroll_item_installment_total")]
		public int? InstallmentTotal { get; set; }

		[Column("criado_por")]
		public long CriadoPor { get; set; }

		[Column("atualizado_por")]
		public long? AtualizadoPor { get; set; }

		[Column("criado_em")]
		public DateTime CriadoEm { get; set; }

		[Column("atualizado_em")]
		public DateTime? AtualizadoEm { get; set; }


		//Criando Relação com a tabelas
        public virtual PayrollEmployee PayrollEmployee { get; set; } = null!;
		// Construtor padrão para EF
		public PayrollItem() { }

		// Construtor com parâmetros
		public PayrollItem(
			long Param_PayrollEmployeeId, 
			string Param_Description, 
			string Param_Type, 
			string Param_Category, 
			long Param_Amount, 
			long? Param_ReferenceId, 
			long? Param_CalculationBasis, 
			string? Param_CalculationDetails, 
			long Param_CriadoPor, 
			long? Param_AtualizadoPor, 
			DateTime Param_CriadoEm, 
			DateTime? Param_AtualizadoEm
		)
		{
			PayrollEmployeeId = Param_PayrollEmployeeId;
			Description = Param_Description;
			Type = Param_Type;
			Category = Param_Category;
			Amount = Param_Amount;
			ReferenceId = Param_ReferenceId;
			CalculationBasis = Param_CalculationBasis;
			CalculationDetails = Param_CalculationDetails;
			CriadoPor = Param_CriadoPor;
			AtualizadoPor = Param_AtualizadoPor;
			CriadoEm = Param_CriadoEm;
			AtualizadoEm = Param_AtualizadoEm;
		}
	}
}
