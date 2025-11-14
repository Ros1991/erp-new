using System.ComponentModel.DataAnnotations;

namespace ERP.Application.DTOs
{
    public class LoginRequestDTO
    {
        [Required(ErrorMessage = "Email, telefone ou CPF é obrigatório")]
        public string Credential { get; set; }

        [Required(ErrorMessage = "Senha é obrigatório")]
        [MinLength(6, ErrorMessage = "Senha deve ter no mínimo 6 caracteres")]
        public string Password { get; set; }
    }
}
