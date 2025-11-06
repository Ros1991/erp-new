using System.ComponentModel.DataAnnotations;

namespace CAU.Application.DTOs.Auth
{
    public class ForgotPasswordRequestDTO
    {
        [Required(ErrorMessage = "Email, Phone ou CPF é obrigatório")]
        public string Credential { get; set; }
    }
}
