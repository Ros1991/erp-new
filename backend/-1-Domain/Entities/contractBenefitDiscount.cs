//base: baseentity.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERP.Domain.Entities
{
	[Table("tb_contract_benefit_discount", Schema = "erp")]
	public class ContractBenefitDiscount
	{
		[Key]
		//@AutoIncrement
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Column("contract_benefit_discount_id")]
		public long ContractBenefitDiscountId { get; set; }

		[Column("contract_id")]
		public long ContractId { get; set; }

		[Column("contract_benefit_discount_description")]
		public string Description { get; set; }

		[Column("contract_benefit_discount_type")]
		public string Type { get; set; }

		[Column("contract_benefit_discount_application")]
		public string Application { get; set; }

		[Column("contract_benefit_discount_amount")]
		public long Amount { get; set; } // Armazenado em centavos

		[Column("contract_benefit_discount_month")]
		public int? Month { get; set; } // Mês (1-12) para benefícios anuais

		[Column("contract_benefit_discount_has_taxes")]
		public bool HasTaxes { get; set; } = false;

		[Column("contract_benefit_discount_is_proportional")]
		public bool IsProportional { get; set; } = true;

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
		// Construtor padrão para EF
		public ContractBenefitDiscount() { }

		// Construtor com parâmetros
		public ContractBenefitDiscount(
			long Param_ContractId, 
			string Param_Description, 
			string Param_Type, 
			string Param_Application, 
			long Param_Amount, 
			long Param_CriadoPor, 
			long? Param_AtualizadoPor, 
			DateTime Param_CriadoEm, 
			DateTime? Param_AtualizadoEm
		)
		{
			ContractId = Param_ContractId;
			Description = Param_Description;
			Type = Param_Type;
			Application = Param_Application;
			Amount = Param_Amount;
			CriadoPor = Param_CriadoPor;
			AtualizadoPor = Param_AtualizadoPor;
			CriadoEm = Param_CriadoEm;
			AtualizadoEm = Param_AtualizadoEm;
		}
	}
}
