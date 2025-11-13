//base: baseentity.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERP.Domain.Entities
{
	[Table("tb_supplier_customer", Schema = "erp")]
	public class SupplierCustomer
	{
		[Key]
		//@AutoIncrement
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Column("supplier_customer_id")]
		public long SupplierCustomerId { get; set; }

		[Column("company_id")]
		public long CompanyId { get; set; }

		[Column("supplier_customer_name")]
		public string Name { get; set; }

		[Column("supplier_customer_document")]
		public string? Document { get; set; }

		[Column("supplier_customer_email")]
		public string? Email { get; set; }

		[Column("supplier_customer_phone")]
		public string? Phone { get; set; }

		[Column("supplier_customer_address")]
		public string? Address { get; set; }

		[Column("supplier_customer_is_active")]
		public bool IsActive { get; set; }

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
		//Parent Relations
        public virtual ICollection<AccountPayableReceivable> AccountPayableReceivableList { get; set; } = new List<AccountPayableReceivable>();
		//Parent Relations
        public virtual ICollection<FinancialTransaction> FinancialTransactionList { get; set; } = new List<FinancialTransaction>();
		// Construtor padrão para EF
		public SupplierCustomer() { }

		// Construtor com parâmetros
		public SupplierCustomer(
			long Param_CompanyId, 
			string Param_Name, 
			string? Param_Document, 
			string? Param_Email, 
			string? Param_Phone, 
			string? Param_Address, 
			bool Param_IsActive, 
			long Param_CriadoPor, 
			long? Param_AtualizadoPor, 
			DateTime Param_CriadoEm, 
			DateTime? Param_AtualizadoEm
		)
		{
			CompanyId = Param_CompanyId;
			Name = Param_Name;
			Document = Param_Document;
			Email = Param_Email;
			Phone = Param_Phone;
			Address = Param_Address;
			IsActive = Param_IsActive;
			CriadoPor = Param_CriadoPor;
			AtualizadoPor = Param_AtualizadoPor;
			CriadoEm = Param_CriadoEm;
			AtualizadoEm = Param_AtualizadoEm;
		}
	}
}
