//base: baseentity.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERP.Domain.Entities
{
	[Table("tb_account_payable_receivable", Schema = "erp")]
	public class AccountPayableReceivable
	{
		[Key]
		//@AutoIncrement
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Column("account_payable_receivable_id")]
		public long AccountPayableReceivableId { get; set; }

		[Column("company_id")]
		public long CompanyId { get; set; }

		[Column("supplier_customer_id")]
		public long? SupplierCustomerId { get; set; }

		[Column("account_payable_receivable_description")]
		public string Description { get; set; }

		[Column("account_payable_receivable_type")]
		public string Type { get; set; }

		[Column("account_payable_receivable_amount")]
		public long Amount { get; set; }

		[Column("account_payable_receivable_due_date")]
		public DateTime DueDate { get; set; }

		[Column("account_payable_receivable_is_paid")]
		public bool IsPaid { get; set; }

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
		//Criando Relação com a tabelas
		public virtual SupplierCustomer SupplierCustomer { get; set; } = null!;
		//Parent Relations
		public virtual ICollection<FinancialTransaction> FinancialTransactionList { get; set; } = new List<FinancialTransaction>();
		// Construtor padrão para EF
		public AccountPayableReceivable() { }

		// Construtor com parâmetros
		public AccountPayableReceivable(
			long Param_CompanyId, 
			long? Param_SupplierCustomerId, 
			string Param_Description, 
			string Param_Type, 
			long Param_Amount, 
			DateTime Param_DueDate, 
			bool Param_IsPaid, 
			long Param_CriadoPor, 
			long? Param_AtualizadoPor, 
			DateTime Param_CriadoEm, 
			DateTime? Param_AtualizadoEm
		)
		{
			CompanyId = Param_CompanyId;
			SupplierCustomerId = Param_SupplierCustomerId;
			Description = Param_Description;
			Type = Param_Type;
			Amount = Param_Amount;
			DueDate = Param_DueDate;
			IsPaid = Param_IsPaid;
			CriadoPor = Param_CriadoPor;
			AtualizadoPor = Param_AtualizadoPor;
			CriadoEm = Param_CriadoEm;
			AtualizadoEm = Param_AtualizadoEm;
		}
	}
}
