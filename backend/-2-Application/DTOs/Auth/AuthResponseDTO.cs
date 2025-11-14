namespace ERP.Application.DTOs
{
    public class AuthResponseDTO
    {
        public long UserId { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Cpf { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public DateTime ExpiresAt { get; set; }
        public DateTime RefreshExpiresAt { get; set; }
    }
}
