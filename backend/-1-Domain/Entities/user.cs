//base: baseentity.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERP.Domain.Entities
{
	[Table("tb_user", Schema = "erp")]
	public class User
	{
		[Key]
		//@AutoIncrement
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Column("user_id")]
		public long UserId { get; set; }

		[Column("user_email")]
		[MaxLength(255)]
		public string? Email { get; set; }

		[Column("user_phone")]
		[MaxLength(20)]
		public string? Phone { get; set; }

		[Column("user_cpf")]
		[MaxLength(11)]
		public string? Cpf { get; set; }

		[Column("user_password_hash")]
		public string PasswordHash { get; set; }

		[Column("user_reset_token")]
		[MaxLength(2048)]
		public string? ResetToken { get; set; }

		[Column("user_reset_token_expires_at")]
		public DateTime? ResetTokenExpiresAt { get; set; }


		//Parent Relations
        public virtual ICollection<CompanyUser> CompanyUserList { get; set; } = new List<CompanyUser>();
		//Parent Relations
        public virtual ICollection<Employee> EmployeeList { get; set; } = new List<Employee>();
		//Parent Relations
        public virtual ICollection<Justification> JustificationList { get; set; } = new List<Justification>();
		//Parent Relations
        public virtual ICollection<PurchaseOrder> PurchaseOrderRequesterList { get; set; } = new List<PurchaseOrder>();
		//Parent Relations
        public virtual ICollection<PurchaseOrder> PurchaseOrderApproverList { get; set; } = new List<PurchaseOrder>();
		//Parent Relations
        public virtual ICollection<TaskComment> TaskCommentList { get; set; } = new List<TaskComment>();
		//Parent Relations
        public virtual ICollection<UserToken> UserTokenList { get; set; } = new List<UserToken>();
		// Construtor padrão para EF
		public User() { }

		// Construtor com parâmetros
		public User(
			string? Param_Email, 
			string? Param_Phone, 
			string? Param_Cpf, 
			string Param_PasswordHash, 
			string? Param_ResetToken, 
			DateTime? Param_ResetTokenExpiresAt
		)
		{
			Email = Param_Email;
			Phone = Param_Phone;
			Cpf = Param_Cpf;
			PasswordHash = Param_PasswordHash;
			ResetToken = Param_ResetToken;
			ResetTokenExpiresAt = Param_ResetTokenExpiresAt;
		}
	}
}
