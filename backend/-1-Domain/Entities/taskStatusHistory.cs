//base: baseentity.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERP.Domain.Entities
{
	[Table("tb_task_status_history", Schema = "erp")]
	public class TaskStatusHistory
	{
		[Key]
		//@AutoIncrement
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Column("task_status_history_id")]
		public long TaskStatusHistoryId { get; set; }

		[Column("task_employee_id")]
		public long TaskEmployeeId { get; set; }

		[Column("task_status_history_old_status")]
		public string OldStatus { get; set; }

		[Column("task_status_history_new_status")]
		public string NewStatus { get; set; }

		[Column("task_status_history_change_reason")]
		public string ChangeReason { get; set; }

		[Column("criado_por")]
		public long CriadoPor { get; set; }

		[Column("atualizado_por")]
		public long AtualizadoPor { get; set; }

		[Column("criado_em")]
		public DateTime CriadoEm { get; set; }

		[Column("atualizado_em")]
		public DateTime AtualizadoEm { get; set; }


		//Criando Relação com a tabelas
        public virtual TaskEmployee TaskEmployee { get; set; } = null!;
		// Construtor padrão para EF
		public TaskStatusHistory() { }

		// Construtor com parâmetros
		public TaskStatusHistory(
			long Param_TaskEmployeeId, 
			string Param_OldStatus, 
			string Param_NewStatus, 
			string Param_ChangeReason, 
			long Param_CriadoPor, 
			long Param_AtualizadoPor, 
			DateTime Param_CriadoEm, 
			DateTime Param_AtualizadoEm
		)
		{
			TaskEmployeeId = Param_TaskEmployeeId;
			OldStatus = Param_OldStatus;
			NewStatus = Param_NewStatus;
			ChangeReason = Param_ChangeReason;
			CriadoPor = Param_CriadoPor;
			AtualizadoPor = Param_AtualizadoPor;
			CriadoEm = Param_CriadoEm;
			AtualizadoEm = Param_AtualizadoEm;
		}
	}
}
