//base: baseentity.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERP.Domain.Entities
{
	[Table("tb_task_employee", Schema = "erp")]
	public class TaskEmployee
	{
		[Key]
		//@AutoIncrement
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Column("task_employee_id")]
		public long TaskEmployeeId { get; set; }

		[Column("task_id")]
		public long TaskId { get; set; }

		[Column("employee_id")]
		public long EmployeeId { get; set; }

		[Column("task_employee_status")]
		public string Status { get; set; }

		[Column("task_employee_estimated_hours")]
		public long? EstimatedHours { get; set; }

		[Column("task_employee_actual_hours")]
		public long? ActualHours { get; set; }

		[Column("task_employee_start_date")]
		public DateTime? StartDate { get; set; }

		[Column("task_employee_end_date")]
		public DateTime? EndDate { get; set; }

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
        public virtual Task Task { get; set; } = null!;
		//Parent Relations
        public virtual ICollection<TaskStatusHistory> TaskStatusHistoryList { get; set; } = new List<TaskStatusHistory>();
		// Construtor padrão para EF
		public TaskEmployee() { }

		// Construtor com parâmetros
		public TaskEmployee(
			long Param_TaskId, 
			long Param_EmployeeId, 
			string Param_Status, 
			long? Param_EstimatedHours, 
			long? Param_ActualHours, 
			DateTime? Param_StartDate, 
			DateTime? Param_EndDate, 
			long Param_CriadoPor, 
			long? Param_AtualizadoPor, 
			DateTime Param_CriadoEm, 
			DateTime? Param_AtualizadoEm
		)
		{
			TaskId = Param_TaskId;
			EmployeeId = Param_EmployeeId;
			Status = Param_Status;
			EstimatedHours = Param_EstimatedHours;
			ActualHours = Param_ActualHours;
			StartDate = Param_StartDate;
			EndDate = Param_EndDate;
			CriadoPor = Param_CriadoPor;
			AtualizadoPor = Param_AtualizadoPor;
			CriadoEm = Param_CriadoEm;
			AtualizadoEm = Param_AtualizadoEm;
		}
	}
}
