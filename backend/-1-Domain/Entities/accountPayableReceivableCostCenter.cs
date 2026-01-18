//base: baseentity.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERP.Domain.Entities
{
	[Table("tb_account_payable_receivable_cost_center", Schema = "erp")]
	public class AccountPayableReceivableCostCenter
	{
		[Key]
		//@AutoIncrement
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Column("account_payable_receivable_cost_center_id")]
		public long AccountPayableReceivableCostCenterId { get; set; }

		[Column("account_payable_receivable_id")]
		public long AccountPayableReceivableId { get; set; }

		[Column("cost_center_id")]
		public long CostCenterId { get; set; }

		[Column("account_payable_receivable_cost_center_amount")]
		public long Amount { get; set; }

		[Column("account_payable_receivable_cost_center_percentage")]
		public decimal Percentage { get; set; }

		[Column("criado_por")]
		public long CriadoPor { get; set; }

		[Column("atualizado_por")]
		public long? AtualizadoPor { get; set; }

		[Column("criado_em")]
		public DateTime CriadoEm { get; set; }

		[Column("atualizado_em")]
		public DateTime? AtualizadoEm { get; set; }


		//Criando Relação com a tabelas
        public virtual CostCenter CostCenter { get; set; } = null!;
		//Criando Relação com a tabelas
        public virtual AccountPayableReceivable AccountPayableReceivable { get; set; } = null!;
		// Construtor padrão para EF
		public AccountPayableReceivableCostCenter() { }

		// Construtor com parâmetros
		public AccountPayableReceivableCostCenter(
			long Param_AccountPayableReceivableId, 
			long Param_CostCenterId, 
			long Param_Amount, 
			decimal Param_Percentage, 
			long Param_CriadoPor, 
			long? Param_AtualizadoPor, 
			DateTime Param_CriadoEm, 
			DateTime? Param_AtualizadoEm
		)
		{
			AccountPayableReceivableId = Param_AccountPayableReceivableId;
			CostCenterId = Param_CostCenterId;
			Amount = Param_Amount;
			Percentage = Param_Percentage;
			CriadoPor = Param_CriadoPor;
			AtualizadoPor = Param_AtualizadoPor;
			CriadoEm = Param_CriadoEm;
			AtualizadoEm = Param_AtualizadoEm;
		}
	}
}
