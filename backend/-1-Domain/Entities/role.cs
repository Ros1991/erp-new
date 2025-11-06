//base: baseentity.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERP.Domain.Entities
{
	[Table("tb_role", Schema = "erp")]
	public class Role
	{
		[Key]
		//@AutoIncrement
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Column("role_id")]
		public long RoleId { get; set; }

		[Column("company_id")]
		public long CompanyId { get; set; }

		[Column("role_name")]
		public string Name { get; set; }

		[Column("role_permissions")]
		public any Permissions { get; set; }

		[Column("criado_por")]
		public long CriadoPor { get; set; }

		[Column("atualizado_por")]
		public long AtualizadoPor { get; set; }

		[Column("criado_em")]
		public DateTime CriadoEm { get; set; }

		[Column("atualizado_em")]
		public DateTime AtualizadoEm { get; set; }


		//Parent Relations
        public virtual ICollection<CompanyUser> CompanyUserList { get; set; } = new List<CompanyUser>();
		// Construtor padrão para EF
		public Role() { }

		// Construtor com parâmetros
		public Role(
			long Param_CompanyId, 
			string Param_Name, 
			any Param_Permissions, 
			long Param_CriadoPor, 
			long Param_AtualizadoPor, 
			DateTime Param_CriadoEm, 
			DateTime Param_AtualizadoEm
		)
		{
			CompanyId = Param_CompanyId;
			Name = Param_Name;
			Permissions = Param_Permissions;
			CriadoPor = Param_CriadoPor;
			AtualizadoPor = Param_AtualizadoPor;
			CriadoEm = Param_CriadoEm;
			AtualizadoEm = Param_AtualizadoEm;
		}
	}
}
