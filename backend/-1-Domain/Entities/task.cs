//base: baseentity.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERP.Domain.Entities
{
	[Table("tb_task", Schema = "erp")]
	public class Task
	{
		[Key]
		//@AutoIncrement
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Column("task_id")]
		public long TaskId { get; set; }

		[Column("company_id")]
		public long CompanyId { get; set; }

		[Column("task_id_parent")]
		public long TaskIdParent { get; set; }

		[Column("task_id_blocking")]
		public long TaskIdBlocking { get; set; }

		[Column("task_title")]
		public string Title { get; set; }

		[Column("task_description")]
		public string Description { get; set; }

		[Column("task_priority")]
		public string Priority { get; set; }

		[Column("task_frequency_days")]
		public long FrequencyDays { get; set; }

		[Column("task_allow_sunday")]
		public bool AllowSunday { get; set; }

		[Column("task_allow_monday")]
		public bool AllowMonday { get; set; }

		[Column("task_allow_tuesday")]
		public bool AllowTuesday { get; set; }

		[Column("task_allow_wednesday")]
		public bool AllowWednesday { get; set; }

		[Column("task_allow_thursday")]
		public bool AllowThursday { get; set; }

		[Column("task_allow_friday")]
		public bool AllowFriday { get; set; }

		[Column("task_allow_saturday")]
		public bool AllowSaturday { get; set; }

		[Column("task_start_date")]
		public DateTime StartDate { get; set; }

		[Column("task_end_date")]
		public DateTime EndDate { get; set; }

		[Column("task_overall_status")]
		public string OverallStatus { get; set; }

		[Column("criado_por")]
		public long CriadoPor { get; set; }

		[Column("atualizado_por")]
		public long AtualizadoPor { get; set; }

		[Column("criado_em")]
		public DateTime CriadoEm { get; set; }

		[Column("atualizado_em")]
		public DateTime AtualizadoEm { get; set; }


		//Criando Relação com a tabelas
        public virtual Company Company { get; set; } = null!;
		//Criando Relação com a tabelas
        public virtual Task Task { get; set; } = null!;
		//Criando Relação com a tabelas
        public virtual Task Task { get; set; } = null!;
		//Parent Relations
        public virtual ICollection<TaskComment> TaskCommentList { get; set; } = new List<TaskComment>();
		//Parent Relations
        public virtual ICollection<TaskEmployee> TaskEmployeeList { get; set; } = new List<TaskEmployee>();
		// Construtor padrão para EF
		public Task() { }

		// Construtor com parâmetros
		public Task(
			long Param_CompanyId, 
			long Param_TaskIdParent, 
			long Param_TaskIdBlocking, 
			string Param_Title, 
			string Param_Description, 
			string Param_Priority, 
			long Param_FrequencyDays, 
			bool Param_AllowSunday, 
			bool Param_AllowMonday, 
			bool Param_AllowTuesday, 
			bool Param_AllowWednesday, 
			bool Param_AllowThursday, 
			bool Param_AllowFriday, 
			bool Param_AllowSaturday, 
			DateTime Param_StartDate, 
			DateTime Param_EndDate, 
			string Param_OverallStatus, 
			long Param_CriadoPor, 
			long Param_AtualizadoPor, 
			DateTime Param_CriadoEm, 
			DateTime Param_AtualizadoEm
		)
		{
			CompanyId = Param_CompanyId;
			TaskIdParent = Param_TaskIdParent;
			TaskIdBlocking = Param_TaskIdBlocking;
			Title = Param_Title;
			Description = Param_Description;
			Priority = Param_Priority;
			FrequencyDays = Param_FrequencyDays;
			AllowSunday = Param_AllowSunday;
			AllowMonday = Param_AllowMonday;
			AllowTuesday = Param_AllowTuesday;
			AllowWednesday = Param_AllowWednesday;
			AllowThursday = Param_AllowThursday;
			AllowFriday = Param_AllowFriday;
			AllowSaturday = Param_AllowSaturday;
			StartDate = Param_StartDate;
			EndDate = Param_EndDate;
			OverallStatus = Param_OverallStatus;
			CriadoPor = Param_CriadoPor;
			AtualizadoPor = Param_AtualizadoPor;
			CriadoEm = Param_CriadoEm;
			AtualizadoEm = Param_AtualizadoEm;
		}
	}
}
