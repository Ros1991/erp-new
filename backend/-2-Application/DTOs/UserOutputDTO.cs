namespace ERP.Application.DTOs
{
    public class UserOutputDTO
    {
		public long UserId { get; set; }
		public string Email { get; set; }
		public string Phone { get; set; }
		public string Cpf { get; set; }
		public string PasswordHash { get; set; }	
		public string ResetToken { get; set; }
		public string ResetTokenExpiresAt { get; set; }
    }
}
