using System.ComponentModel.DataAnnotations;

namespace CAU.Application.DTOs.Auth
{
    public class ResetPasswordRequestDTO
    {
        [Required(ErrorMessage = "Token é obrigatório")]
        public string Token { get; set; }

        [Required(ErrorMessage = "Password é obrigatório")]
        [MinLength(6, ErrorMessage = "Password deve ter no mínimo 6 caracteres")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "ConfirmPassword é obrigatório")]
        [Compare("NewPassword", ErrorMessage = "Senhas não conferem")]
        public string ConfirmPassword { get; set; }
    }
}
