//base: baseentity.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERP.Domain.Entities
{
	[Table("tb_default_cost_center_distribution", Schema = "erp")]
	public class DefaultCostCenterDistribution
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Column("default_cost_center_distribution_id")]
		public long DefaultCostCenterDistributionId { get; set; }

		[Column("company_id")]
		public long CompanyId { get; set; }

		[Column("cost_center_id")]
		public long CostCenterId { get; set; }

		[Column("default_cost_center_distribution_percentage")]
		public decimal Percentage { get; set; }

		[Column("criado_por")]
		public long CriadoPor { get; set; }

		[Column("atualizado_por")]
		public long? AtualizadoPor { get; set; }

		[Column("criado_em")]
		public DateTime CriadoEm { get; set; }

		[Column("atualizado_em")]
		public DateTime? AtualizadoEm { get; set; }

		// Relações
		public virtual Company Company { get; set; } = null!;
		public virtual CostCenter CostCenter { get; set; } = null!;

		// Construtor padrão para EF
		public DefaultCostCenterDistribution() { }
	}
}
