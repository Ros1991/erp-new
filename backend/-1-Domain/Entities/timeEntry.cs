//base: baseentity.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERP.Domain.Entities
{
	[Table("tb_time_entry", Schema = "erp")]
	public class TimeEntry
	{
		[Key]
		//@AutoIncrement
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Column("time_entry_id")]
		public long TimeEntryId { get; set; }

		[Column("employee_id")]
		public long EmployeeId { get; set; }

		[Column("time_entry_type")]
		public string Type { get; set; }

		[Column("time_entry_timestamp")]
		public DateTime Timestamp { get; set; }

		[Column("time_entry_latitude")]
		public long? Latitude { get; set; }

		[Column("time_entry_longitude")]
		public long? Longitude { get; set; }

		[Column("location_id")]
		public long? LocationId { get; set; }

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
		public TimeEntry() { }

		// Construtor com parâmetros
		public TimeEntry(
			long Param_EmployeeId, 
			string Param_Type, 
			DateTime Param_Timestamp, 
			long? Param_Latitude, 
			long? Param_Longitude, 
			long? Param_LocationId, 
			long Param_CriadoPor, 
			long? Param_AtualizadoPor, 
			DateTime Param_CriadoEm, 
			DateTime? Param_AtualizadoEm
		)
		{
			EmployeeId = Param_EmployeeId;
			Type = Param_Type;
			Timestamp = Param_Timestamp;
			Latitude = Param_Latitude;
			Longitude = Param_Longitude;
			LocationId = Param_LocationId;
			CriadoPor = Param_CriadoPor;
			AtualizadoPor = Param_AtualizadoPor;
			CriadoEm = Param_CriadoEm;
			AtualizadoEm = Param_AtualizadoEm;
		}
	}
}
