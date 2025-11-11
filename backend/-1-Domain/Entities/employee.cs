//base: baseentity.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERP.Domain.Entities
{
	[Table("tb_employee", Schema = "erp")]
	public class Employee
	{
		[Key]
		//@AutoIncrement
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Column("employee_id")]
		public long EmployeeId { get; set; }

		[Column("company_id")]
		public long CompanyId { get; set; }

		[Column("user_id")]
		public long UserId { get; set; }

		[Column("employee_id_manager")]
		public long EmployeeIdManager { get; set; }

		[Column("employee_nickname")]
		public string Nickname { get; set; }

		[Column("employee_full_name")]
		public string FullName { get; set; }

		[Column("employee_email")]
		public string Email { get; set; }

		[Column("employee_phone")]
		public string Phone { get; set; }

		[Column("employee_cpf")]
		public string Cpf { get; set; }

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
        public virtual Employee EmployeeManager { get; set; } = null!;
		//Criando Relação com a tabelas
        public virtual User User { get; set; } = null!;
		//Parent Relations
        public virtual ICollection<Contract> ContractList { get; set; } = new List<Contract>();
		//Parent Relations
        public virtual ICollection<EmployeeAllowedLocation> EmployeeAllowedLocationList { get; set; } = new List<EmployeeAllowedLocation>();
        //Parent Relations
        public virtual ICollection<Justification> JustificationList { get; set; } = new List<Justification>();
        //Parent Relations
        public virtual ICollection<Employee> EmployeeManagerList { get; set; } = new List<Employee>();
        //Parent Relations
        public virtual ICollection<LoanAdvance> LoanAdvanceList { get; set; } = new List<LoanAdvance>();
		//Parent Relations
        public virtual ICollection<PayrollEmployee> PayrollEmployeeList { get; set; } = new List<PayrollEmployee>();
		//Parent Relations
        public virtual ICollection<TaskEmployee> TaskEmployeeList { get; set; } = new List<TaskEmployee>();
		//Parent Relations
        public virtual ICollection<TimeEntry> TimeEntryList { get; set; } = new List<TimeEntry>();
		// Construtor padrão para EF
		public Employee() { }

		// Construtor com parâmetros
		public Employee(
			long Param_CompanyId, 
			long Param_UserId, 
			long Param_EmployeeIdManager, 
			string Param_Nickname, 
			string Param_FullName, 
			string Param_Email, 
			string Param_Phone, 
			string Param_Cpf, 
			long Param_CriadoPor, 
			long Param_AtualizadoPor, 
			DateTime Param_CriadoEm, 
			DateTime Param_AtualizadoEm
		)
		{
			CompanyId = Param_CompanyId;
			UserId = Param_UserId;
			EmployeeIdManager = Param_EmployeeIdManager;
			Nickname = Param_Nickname;
			FullName = Param_FullName;
			Email = Param_Email;
			Phone = Param_Phone;
			Cpf = Param_Cpf;
			CriadoPor = Param_CriadoPor;
			AtualizadoPor = Param_AtualizadoPor;
			CriadoEm = Param_CriadoEm;
			AtualizadoEm = Param_AtualizadoEm;
		}
	}
}
