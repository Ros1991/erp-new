//base: baseentity.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERP.Domain.Entities
{
	[Table("tb_task_comment", Schema = "erp")]
	public class TaskComment
	{
		[Key]
		//@AutoIncrement
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Column("task_comment_id")]
		public long TaskCommentId { get; set; }

		[Column("task_id")]
		public long TaskId { get; set; }

		[Column("user_id")]
		public long UserId { get; set; }

		[Column("task_comment_comment")]
		public string Comment { get; set; }

		[Column("task_comment_attachment_url")]
		[MaxLength(2048)]
		public string? AttachmentUrl { get; set; }

		[Column("criado_por")]
		public long CriadoPor { get; set; }

		[Column("atualizado_por")]
		public long? AtualizadoPor { get; set; }

		[Column("criado_em")]
		public DateTime CriadoEm { get; set; }

		[Column("atualizado_em")]
		public DateTime? AtualizadoEm { get; set; }


		//Criando Relação com a tabelas
        public virtual Task Task { get; set; } = null!;
		//Criando Relação com a tabelas
        public virtual User User { get; set; } = null!;
		// Construtor padrão para EF
		public TaskComment() { }

		// Construtor com parâmetros
		public TaskComment(
			long Param_TaskId, 
			long Param_UserId, 
			string Param_Comment, 
			string? Param_AttachmentUrl, 
			long Param_CriadoPor, 
			long? Param_AtualizadoPor, 
			DateTime Param_CriadoEm, 
			DateTime? Param_AtualizadoEm
		)
		{
			TaskId = Param_TaskId;
			UserId = Param_UserId;
			Comment = Param_Comment;
			AttachmentUrl = Param_AttachmentUrl;
			CriadoPor = Param_CriadoPor;
			AtualizadoPor = Param_AtualizadoPor;
			CriadoEm = Param_CriadoEm;
			AtualizadoEm = Param_AtualizadoEm;
		}
	}
}
