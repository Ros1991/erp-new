//base: baseentity.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERP.Domain.Entities
{
	[Table("tb_location", Schema = "erp")]
	public class Location
	{
		[Key]
		//@AutoIncrement
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Column("location_id")]
		public long LocationId { get; set; }

		[Column("company_id")]
		public long CompanyId { get; set; }

		[Column("location_name")]
		public string Name { get; set; }

		[Column("location_address")]
		public string? Address { get; set; }

		[Column("location_latitude")]
		public long Latitude { get; set; }

		[Column("location_longitude")]
		public long Longitude { get; set; }

		[Column("location_radius_meters")]
		public long RadiusMeters { get; set; }

		[Column("location_is_active")]
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
        public virtual ICollection<EmployeeAllowedLocation> EmployeeAllowedLocationList { get; set; } = new List<EmployeeAllowedLocation>();
		//Parent Relations
        public virtual ICollection<TimeEntry> TimeEntryList { get; set; } = new List<TimeEntry>();
		// Construtor padrão para EF
		public Location() { }

		// Construtor com parâmetros
		public Location(
			long Param_CompanyId, 
			string Param_Name, 
			string? Param_Address, 
			long Param_Latitude, 
			long Param_Longitude, 
			long Param_RadiusMeters, 
			bool Param_IsActive, 
			long Param_CriadoPor, 
			long? Param_AtualizadoPor, 
			DateTime Param_CriadoEm, 
			DateTime? Param_AtualizadoEm
		)
		{
			CompanyId = Param_CompanyId;
			Name = Param_Name;
			Address = Param_Address;
			Latitude = Param_Latitude;
			Longitude = Param_Longitude;
			RadiusMeters = Param_RadiusMeters;
			IsActive = Param_IsActive;
			CriadoPor = Param_CriadoPor;
			AtualizadoPor = Param_AtualizadoPor;
			CriadoEm = Param_CriadoEm;
			AtualizadoEm = Param_AtualizadoEm;
		}
	}
}
