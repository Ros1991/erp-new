using System.ComponentModel.DataAnnotations;

namespace ERP.Application.DTOs
{
    public class EmployeeInputDTO
    {
        public long? UserId { get; set; }
        
        public long? EmployeeIdManager { get; set; }
        
        [Required(ErrorMessage = "Apelido é obrigatório")]
        [StringLength(255, ErrorMessage = "Apelido deve ter no máximo 255 caracteres")]
        public string Nickname { get; set; }
        
        [StringLength(255, ErrorMessage = "Nome completo deve ter no máximo 255 caracteres")]
        public string? FullName { get; set; }
        
        [StringLength(255, ErrorMessage = "Email deve ter no máximo 255 caracteres")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        public string? Email { get; set; }
        
        [StringLength(20, ErrorMessage = "Telefone deve ter no máximo 20 caracteres")]
        public string? Phone { get; set; }
        
        [StringLength(11, ErrorMessage = "CPF deve ter exatamente 11 caracteres")]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "CPF deve conter apenas números")]
        public string? Cpf { get; set; }
        
        // Imagem de perfil como Base64 para upload via API
        public string? ProfileImageBase64 { get; set; }
    }
}
