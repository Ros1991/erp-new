//base: baseentity.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERP.Domain.Entities
{
	[Table("tb_justification", Schema = "erp")]
	public class Justification
	{
		[Key]
		//@AutoIncrement
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Column("justification_id")]
		public long JustificationId { get; set; }

		[Column("employee_id")]
		public long EmployeeId { get; set; }

		[Column("justification_reference_date")]
		public DateTime ReferenceDate { get; set; }

		[Column("justification_reason")]
		public string Reason { get; set; }

		[Column("justification_attachment_url")]
		[MaxLength(2048)]
		public string? AttachmentUrl { get; set; }

		[Column("justification_hours_granted")]
		public long? HoursGranted { get; set; }

		[Column("user_id_approver")]
		public long? UserIdApprover { get; set; }

		[Column("justification_status")]
		public string Status { get; set; }

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
        public virtual User User { get; set; } = null!;
		// Construtor padrão para EF
		public Justification() { }

		// Construtor com parâmetros
		public Justification(
			long Param_EmployeeId, 
			DateTime Param_ReferenceDate, 
			string Param_Reason, 
			string? Param_AttachmentUrl, 
			long? Param_HoursGranted, 
			long? Param_UserIdApprover, 
			string Param_Status, 
			long Param_CriadoPor, 
			long? Param_AtualizadoPor, 
			DateTime Param_CriadoEm, 
			DateTime? Param_AtualizadoEm
		)
		{
			EmployeeId = Param_EmployeeId;
			ReferenceDate = Param_ReferenceDate;
			Reason = Param_Reason;
			AttachmentUrl = Param_AttachmentUrl;
			HoursGranted = Param_HoursGranted;
			UserIdApprover = Param_UserIdApprover;
			Status = Param_Status;
			CriadoPor = Param_CriadoPor;
			AtualizadoPor = Param_AtualizadoPor;
			CriadoEm = Param_CriadoEm;
			AtualizadoEm = Param_AtualizadoEm;
		}
	}
}
