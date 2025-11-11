//base: baseentity.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERP.Domain.Entities
{
	[Table("tb_cost_center", Schema = "erp")]
	public class CostCenter
	{
		[Key]
		//@AutoIncrement
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Column("cost_center_id")]
		public long CostCenterId { get; set; }

		[Column("company_id")]
		public long CompanyId { get; set; }

		[Column("cost_center_name")]
		public string Name { get; set; }

		[Column("cost_center_description")]
		public string? Description { get; set; }

		[Column("cost_center_is_active")]
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
        public virtual ICollection<ContractCostCenter> ContractCostCenterList { get; set; } = new List<ContractCostCenter>();
		//Parent Relations
        public virtual ICollection<TransactionCostCenter> TransactionCostCenterList { get; set; } = new List<TransactionCostCenter>();
		// Construtor padrão para EF
		public CostCenter() { }

		// Construtor com parâmetros
		public CostCenter(
			long Param_CompanyId, 
			string Param_Name, 
			string? Param_Description, 
			bool Param_IsActive, 
			long Param_CriadoPor, 
			long? Param_AtualizadoPor, 
			DateTime Param_CriadoEm, 
			DateTime? Param_AtualizadoEm
		)
		{
			CompanyId = Param_CompanyId;
			Name = Param_Name;
			Description = Param_Description;
			IsActive = Param_IsActive;
			CriadoPor = Param_CriadoPor;
			AtualizadoPor = Param_AtualizadoPor;
			CriadoEm = Param_CriadoEm;
			AtualizadoEm = Param_AtualizadoEm;
		}
	}
}
