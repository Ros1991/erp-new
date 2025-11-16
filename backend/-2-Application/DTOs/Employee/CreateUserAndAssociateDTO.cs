using System.ComponentModel.DataAnnotations;

namespace ERP.Application.DTOs
{
    public class CreateUserAndAssociateDTO
    {
        [EmailAddress(ErrorMessage = "Email inválido")]
        public string? Email { get; set; }
        
        [StringLength(20, ErrorMessage = "Telefone deve ter no máximo 20 caracteres")]
        public string? Phone { get; set; }
        
        [StringLength(11, MinimumLength = 11, ErrorMessage = "CPF deve ter 11 dígitos")]
        public string? Cpf { get; set; }
        
        [Required(ErrorMessage = "Senha é obrigatória")]
        [MinLength(6, ErrorMessage = "Senha deve ter no mínimo 6 caracteres")]
        public string Password { get; set; }
        
        [Required(ErrorMessage = "Cargo é obrigatório")]
        public long RoleId { get; set; }
    }
}
