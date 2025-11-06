using System.ComponentModel.DataAnnotations;

namespace ERP.Application.DTOs.Auth
{
    public class LoginRequestDTO
    {
        [Required(ErrorMessage = "Email, Phone ou CPF é obrigatório")]
        public string Credential { get; set; }

        [Required(ErrorMessage = "Password é obrigatório")]
        [MinLength(6, ErrorMessage = "Password deve ter no mínimo 6 caracteres")]
        public string Password { get; set; }
    }
}
