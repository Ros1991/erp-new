//base: baseentity.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERP.Domain.Entities
{
	[Table("tb_contract_cost_center", Schema = "erp")]
	public class ContractCostCenter
	{
		[Key]
		//@AutoIncrement
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Column("contract_cost_center_id")]
		public long ContractCostCenterId { get; set; }

		[Column("contract_id")]
		public long ContractId { get; set; }

		[Column("cost_center_id")]
		public long CostCenterId { get; set; }

		[Column("contract_cost_center_percentage")]
		public decimal Percentage { get; set; }

		[Column("criado_por")]
		public long CriadoPor { get; set; }

		[Column("atualizado_por")]
		public long? AtualizadoPor { get; set; }

		[Column("criado_em")]
		public DateTime CriadoEm { get; set; }

		[Column("atualizado_em")]
		public DateTime? AtualizadoEm { get; set; }


		//Criando Relação com a tabelas
        public virtual Contract Contract { get; set; } = null!;
		//Criando Relação com a tabelas
        public virtual CostCenter CostCenter { get; set; } = null!;
		// Construtor padrão para EF
		public ContractCostCenter() { }

		// Construtor com parâmetros
		public ContractCostCenter(
			long Param_ContractId, 
			long Param_CostCenterId, 
			decimal Param_Percentage, 
			long Param_CriadoPor, 
			long? Param_AtualizadoPor, 
			DateTime Param_CriadoEm, 
			DateTime? Param_AtualizadoEm
		)
		{
			ContractId = Param_ContractId;
			CostCenterId = Param_CostCenterId;
			Percentage = Param_Percentage;
			CriadoPor = Param_CriadoPor;
			AtualizadoPor = Param_AtualizadoPor;
			CriadoEm = Param_CriadoEm;
			AtualizadoEm = Param_AtualizadoEm;
		}
	}
}
