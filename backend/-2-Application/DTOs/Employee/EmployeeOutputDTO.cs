namespace ERP.Application.DTOs.Employee
{
    public class EmployeeOutputDTO
    {
        public long EmployeeId { get; set; }
        public long CompanyId { get; set; }
        public long? UserId { get; set; }
        public long? EmployeeIdManager { get; set; }
        public string Nickname { get; set; }
        public string FullName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Cpf { get; set; }
        
        // Imagem de perfil como Base64 para download via API
        public string? ProfileImageBase64 { get; set; }
        
        // Dados do manager (se houver)
        public string? ManagerNickname { get; set; }
        public string? ManagerFullName { get; set; }
        
        // Dados do usuário vinculado (se houver)
        public string? UserEmail { get; set; }
        
        // Auditoria
        public long CriadoPor { get; set; }
        public long? AtualizadoPor { get; set; }
        public DateTime CriadoEm { get; set; }
        public DateTime? AtualizadoEm { get; set; }
    }
}
