namespace ERP.Application.DTOs
{
    public class UserSearchResultDTO
    {
        public long? UserId { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Cpf { get; set; }
        public bool HasCompanyAccess { get; set; }
        public long? CurrentRoleId { get; set; }
        public string? CurrentRoleName { get; set; }
        public bool? CurrentRoleIsSystem { get; set; }
    }
}
