//base: baseentity.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CAU.Domain.Entities
{
	[Table("tb_company_user", Schema = "erp")]
	public class CompanyUser
	{
		[Key]
		//@AutoIncrement
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Column("company_user_id")]
		public long CompanyUserId { get; set; }

		[Column("company_id")]
		public long CompanyId { get; set; }

		[Column("user_id")]
		public long UserId { get; set; }

		[Column("role_id")]
		public long RoleId { get; set; }

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
        public virtual Role Role { get; set; } = null!;
		//Criando Relação com a tabelas
        public virtual User User { get; set; } = null!;
		// Construtor padrão para EF
		public CompanyUser() { }

		// Construtor com parâmetros
		public CompanyUser(
			long Param_CompanyId, 
			long Param_UserId, 
			long Param_RoleId, 
			long Param_CriadoPor, 
			long Param_AtualizadoPor, 
			DateTime Param_CriadoEm, 
			DateTime Param_AtualizadoEm
		)
		{
			CompanyId = Param_CompanyId;
			UserId = Param_UserId;
			RoleId = Param_RoleId;
			CriadoPor = Param_CriadoPor;
			AtualizadoPor = Param_AtualizadoPor;
			CriadoEm = Param_CriadoEm;
			AtualizadoEm = Param_AtualizadoEm;
		}
	}
}
