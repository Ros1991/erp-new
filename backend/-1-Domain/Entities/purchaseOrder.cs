//base: baseentity.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CAU.Domain.Entities
{
	[Table("tb_purchase_order", Schema = "erp")]
	public class PurchaseOrder
	{
		[Key]
		//@AutoIncrement
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Column("purchase_order_id")]
		public long PurchaseOrderId { get; set; }

		[Column("company_id")]
		public long CompanyId { get; set; }

		[Column("user_id_requester")]
		public long UserIdRequester { get; set; }

		[Column("user_id_approver")]
		public long UserIdApprover { get; set; }

		[Column("purchase_order_description")]
		public string Description { get; set; }

		[Column("purchase_order_total_amount")]
		public long TotalAmount { get; set; }

		[Column("purchase_order_status")]
		public string Status { get; set; }

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
		//Criando Relação com a tabelas
        public virtual User User { get; set; } = null!;
		//Criando Relação com a tabelas
        public virtual User User { get; set; } = null!;
		//Parent Relations
        public virtual ICollection<FinancialTransaction> FinancialTransactionList { get; set; } = new List<FinancialTransaction>();
		// Construtor padrão para EF
		public PurchaseOrder() { }

		// Construtor com parâmetros
		public PurchaseOrder(
			long Param_CompanyId, 
			long Param_UserIdRequester, 
			long Param_UserIdApprover, 
			string Param_Description, 
			long Param_TotalAmount, 
			string Param_Status, 
			long Param_CriadoPor, 
			long Param_AtualizadoPor, 
			DateTime Param_CriadoEm, 
			DateTime Param_AtualizadoEm
		)
		{
			CompanyId = Param_CompanyId;
			UserIdRequester = Param_UserIdRequester;
			UserIdApprover = Param_UserIdApprover;
			Description = Param_Description;
			TotalAmount = Param_TotalAmount;
			Status = Param_Status;
			CriadoPor = Param_CriadoPor;
			AtualizadoPor = Param_AtualizadoPor;
			CriadoEm = Param_CriadoEm;
			AtualizadoEm = Param_AtualizadoEm;
		}
	}
}
