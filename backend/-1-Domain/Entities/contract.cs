//base: baseentity.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERP.Domain.Entities
{
	[Table("tb_contract", Schema = "erp")]
	public class Contract
	{
		[Key]
		//@AutoIncrement
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Column("contract_id")]
		public long ContractId { get; set; }

		[Column("employee_id")]
		public long EmployeeId { get; set; }

		[Column("contract_type")]
		public string Type { get; set; }

		[Column("contract_value")]
		public long Value { get; set; }

		[Column("contract_is_payroll")]
		public bool IsPayroll { get; set; }

		[Column("contract_has_inss")]
		public bool HasInss { get; set; }

		[Column("contract_has_irrf")]
		public bool HasIrrf { get; set; }

		[Column("contract_has_fgts")]
		public bool HasFgts { get; set; }

		[Column("contract_start_date")]
		public DateTime StartDate { get; set; }

		[Column("contract_end_date")]
		public DateTime? EndDate { get; set; }

		[Column("contract_weekly_hours")]
		public long? WeeklyHours { get; set; }

		[Column("contract_is_active")]
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
        public virtual Employee Employee { get; set; } = null!;
		//Parent Relations
        public virtual ICollection<ContractBenefitDiscount> ContractBenefitDiscountList { get; set; } = new List<ContractBenefitDiscount>();
		//Parent Relations
        public virtual ICollection<ContractCostCenter> ContractCostCenterList { get; set; } = new List<ContractCostCenter>();
		// Construtor padrão para EF
		public Contract() { }

		// Construtor com parâmetros
		public Contract(
			long Param_EmployeeId, 
			string Param_Type, 
			long Param_Value, 
			bool Param_IsPayroll, 
			bool Param_HasInss, 
			bool Param_HasIrrf, 
			bool Param_HasFgts, 
			DateTime Param_StartDate, 
			DateTime? Param_EndDate, 
			long? Param_WeeklyHours, 
			bool Param_IsActive, 
			long Param_CriadoPor, 
			long? Param_AtualizadoPor, 
			DateTime Param_CriadoEm, 
			DateTime? Param_AtualizadoEm
		)
		{
			EmployeeId = Param_EmployeeId;
			Type = Param_Type;
			Value = Param_Value;
			IsPayroll = Param_IsPayroll;
			HasInss = Param_HasInss;
			HasIrrf = Param_HasIrrf;
			HasFgts = Param_HasFgts;
			StartDate = Param_StartDate;
			EndDate = Param_EndDate;
			WeeklyHours = Param_WeeklyHours;
			IsActive = Param_IsActive;
			CriadoPor = Param_CriadoPor;
			AtualizadoPor = Param_AtualizadoPor;
			CriadoEm = Param_CriadoEm;
			AtualizadoEm = Param_AtualizadoEm;
		}
	}
}
