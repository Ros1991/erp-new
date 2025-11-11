//base: baseentity.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERP.Domain.Entities
{
	[Table("tb_employee_allowed_location", Schema = "erp")]
	public class EmployeeAllowedLocation
	{
		[Key]
		//@AutoIncrement
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Column("employee_allowed_location_id")]
		public long EmployeeAllowedLocationId { get; set; }

		[Column("employee_id")]
		public long EmployeeId { get; set; }

		[Column("location_id")]
		public long LocationId { get; set; }

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
		//Criando Relação com a tabelas
        public virtual Location Location { get; set; } = null!;
		// Construtor padrão para EF
		public EmployeeAllowedLocation() { }

		// Construtor com parâmetros
		public EmployeeAllowedLocation(
			long Param_EmployeeId, 
			long Param_LocationId, 
			long Param_CriadoPor, 
			long? Param_AtualizadoPor, 
			DateTime Param_CriadoEm, 
			DateTime? Param_AtualizadoEm
		)
		{
			EmployeeId = Param_EmployeeId;
			LocationId = Param_LocationId;
			CriadoPor = Param_CriadoPor;
			AtualizadoPor = Param_AtualizadoPor;
			CriadoEm = Param_CriadoEm;
			AtualizadoEm = Param_AtualizadoEm;
		}
	}
}
