using System.ComponentModel.DataAnnotations;

namespace CAU.Application.DTOs.Auth
{
    public class RegisterRequestDTO
    {
        [StringLength(255, ErrorMessage = "Email deve ter no máximo 255 caracteres")]
        public string? Email { get; set; }

        [StringLength(20, ErrorMessage = "Phone deve ter no máximo 20 caracteres")]
        public string? Phone { get; set; }

        [StringLength(11, ErrorMessage = "CPF deve ter 11 caracteres")]
        public string? Cpf { get; set; }

        [Required(ErrorMessage = "Password é obrigatório")]
        [MinLength(6, ErrorMessage = "Password deve ter no mínimo 6 caracteres")]
        public string Password { get; set; }

        [Required(ErrorMessage = "ConfirmPassword é obrigatório")]
        [Compare("Password", ErrorMessage = "Senhas não conferem")]
        public string ConfirmPassword { get; set; }
    }
}
