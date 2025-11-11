//base: baseentity.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERP.Domain.Entities
{
	[Table("tb_company", Schema = "erp")]
	public class Company
	{
		[Key]
		//@AutoIncrement
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Column("company_id")]
		public long CompanyId { get; set; }

		[Column("company_name")]
		public string Name { get; set; }

		[Column("company_document")]
		public string Document { get; set; }

		[Column("user_id")]
		public long UserId { get; set; }

		[Column("criado_por")]
		public long CriadoPor { get; set; }

		[Column("atualizado_por")]
		public long? AtualizadoPor { get; set; }

		[Column("criado_em")]
		public DateTime CriadoEm { get; set; }

		[Column("atualizado_em")]
		public DateTime? AtualizadoEm { get; set; }


		//Parent Relations
		public virtual ICollection<Account> AccountList { get; set; } = new List<Account>();
		//Parent Relations
		public virtual ICollection<AccountPayableReceivable> AccountPayableReceivableList { get; set; } = new List<AccountPayableReceivable>();
		//Parent Relations
		public virtual ICollection<CompanySetting> CompanySettingList { get; set; } = new List<CompanySetting>();
		//Parent Relations
		public virtual ICollection<CompanyUser> CompanyUserList { get; set; } = new List<CompanyUser>();
		//Parent Relations
		public virtual ICollection<CostCenter> CostCenterList { get; set; } = new List<CostCenter>();
		//Parent Relations
		public virtual ICollection<Employee> EmployeeList { get; set; } = new List<Employee>();
		//Parent Relations
		public virtual ICollection<FinancialTransaction> FinancialTransactionList { get; set; } = new List<FinancialTransaction>();
		//Parent Relations
		public virtual ICollection<Location> LocationList { get; set; } = new List<Location>();
		//Parent Relations
		public virtual ICollection<Payroll> PayrollList { get; set; } = new List<Payroll>();
		//Parent Relations
		public virtual ICollection<PurchaseOrder> PurchaseOrderList { get; set; } = new List<PurchaseOrder>();
		//Parent Relations
		public virtual ICollection<Task> TaskList { get; set; } = new List<Task>();
		
		// Construtor padrão para EF
		public Company() { }

		// Construtor com parâmetros
		public Company(
			string Param_Name, 
			string Param_Document, 
			long Param_UserId, 
			long Param_CriadoPor, 
			long? Param_AtualizadoPor, 
			DateTime Param_CriadoEm, 
			DateTime? Param_AtualizadoEm
		)
		{
			Name = Param_Name;
			Document = Param_Document;
			UserId = Param_UserId;
			CriadoPor = Param_CriadoPor;
			AtualizadoPor = Param_AtualizadoPor;
			CriadoEm = Param_CriadoEm;
			AtualizadoEm = Param_AtualizadoEm;
		}
	}
}
