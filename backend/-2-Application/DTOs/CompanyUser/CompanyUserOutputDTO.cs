namespace ERP.Application.DTOs
{
    public class CompanyUserOutputDTO
    {
        public long CompanyUserId { get; set; }
        public long CompanyId { get; set; }
        public long UserId { get; set; }
        public long? RoleId { get; set; }
        
        // Dados relacionados do User
        public string? UserEmail { get; set; }
        public string? UserPhone { get; set; }
        public string? UserCpf { get; set; }
        
        // Dados relacionados do Role
        public string? RoleName { get; set; }
        public bool? RoleIsSystem { get; set; }
        
        public long CriadoPor { get; set; }
        public long? AtualizadoPor { get; set; }
        public DateTime CriadoEm { get; set; }
        public DateTime? AtualizadoEm { get; set; }
    }
}
