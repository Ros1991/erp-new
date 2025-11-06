using System.ComponentModel.DataAnnotations;

namespace ERP.Application.DTOs.Auth
{
    public class ForgotPasswordRequestDTO
    {
        [Required(ErrorMessage = "Email, Phone ou CPF é obrigatório")]
        public string Credential { get; set; }
    }
}
