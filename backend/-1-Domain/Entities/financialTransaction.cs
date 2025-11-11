//base: baseentity.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERP.Domain.Entities
{
	[Table("tb_financial_transaction", Schema = "erp")]
	public class FinancialTransaction
	{
		[Key]
		//@AutoIncrement
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Column("financial_transaction_id")]
		public long FinancialTransactionId { get; set; }

		[Column("company_id")]
		public long CompanyId { get; set; }

		[Column("account_id")]
		public long AccountId { get; set; }

		[Column("purchase_order_id")]
		public long? PurchaseOrderId { get; set; }

		[Column("account_payable_receivable_id")]
		public long? AccountPayableReceivableId { get; set; }

		[Column("financial_transaction_description")]
		public string Description { get; set; }

		[Column("financial_transaction_type")]
		public string Type { get; set; }

		[Column("financial_transaction_amount")]
		public long Amount { get; set; }

		[Column("financial_transaction_transaction_date")]
		public DateTime TransactionDate { get; set; }

		[Column("criado_por")]
		public long CriadoPor { get; set; }

		[Column("atualizado_por")]
		public long? AtualizadoPor { get; set; }

		[Column("criado_em")]
		public DateTime CriadoEm { get; set; }

		[Column("atualizado_em")]
		public DateTime? AtualizadoEm { get; set; }


		//Criando Relação com a tabelas
        public virtual Account Account { get; set; } = null!;
		//Criando Relação com a tabelas
        public virtual AccountPayableReceivable AccountPayableReceivable { get; set; } = null!;
		//Criando Relação com a tabelas
        public virtual Company Company { get; set; } = null!;
		//Criando Relação com a tabelas
        public virtual PurchaseOrder PurchaseOrder { get; set; } = null!;
		//Parent Relations
        public virtual ICollection<TransactionCostCenter> TransactionCostCenterList { get; set; } = new List<TransactionCostCenter>();
		// Construtor padrão para EF
		public FinancialTransaction() { }

		// Construtor com parâmetros
		public FinancialTransaction(
			long Param_CompanyId, 
			long Param_AccountId, 
			long? Param_PurchaseOrderId, 
			long? Param_AccountPayableReceivableId, 
			string Param_Description, 
			string Param_Type, 
			long Param_Amount, 
			DateTime Param_TransactionDate, 
			long Param_CriadoPor, 
			long? Param_AtualizadoPor, 
			DateTime Param_CriadoEm, 
			DateTime? Param_AtualizadoEm
		)
		{
			CompanyId = Param_CompanyId;
			AccountId = Param_AccountId;
			PurchaseOrderId = Param_PurchaseOrderId;
			AccountPayableReceivableId = Param_AccountPayableReceivableId;
			Description = Param_Description;
			Type = Param_Type;
			Amount = Param_Amount;
			TransactionDate = Param_TransactionDate;
			CriadoPor = Param_CriadoPor;
			AtualizadoPor = Param_AtualizadoPor;
			CriadoEm = Param_CriadoEm;
			AtualizadoEm = Param_AtualizadoEm;
		}
	}
}
