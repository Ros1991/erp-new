//base: baseentity.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERP.Domain.Entities
{
	[Table("tb_account", Schema = "erp")]
	public class Account
	{
		[Key]
		//@AutoIncrement
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Column("account_id")]
		public long AccountId { get; set; }

		[Column("company_id")]
		public long CompanyId { get; set; }

		[Column("account_name")]
		public string Name { get; set; }

		[Column("account_type")]
		public string Type { get; set; }

		[Column("account_initial_balance")]
		public long InitialBalance { get; set; }

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
		public virtual ICollection<FinancialTransaction> FinancialTransactionList { get; set; } = new List<FinancialTransaction>();
		// Construtor padrão para EF
		public Account() { }

		// Construtor com parâmetros
		public Account(
			long Param_CompanyId, 
			string Param_Name, 
			string Param_Type, 
			long Param_InitialBalance, 
			long Param_CriadoPor, 
			long? Param_AtualizadoPor, 
			DateTime Param_CriadoEm, 
			DateTime? Param_AtualizadoEm
		)
		{
			CompanyId = Param_CompanyId;
			Name = Param_Name;
			Type = Param_Type;
			InitialBalance = Param_InitialBalance;
			CriadoPor = Param_CriadoPor;
			AtualizadoPor = Param_AtualizadoPor;
			CriadoEm = Param_CriadoEm;
			AtualizadoEm = Param_AtualizadoEm;
		}
	}
}
