using System.ComponentModel.DataAnnotations;

namespace ERP.Application.DTOs
{
    public class ForgotPasswordRequestDTO
    {
        [Required(ErrorMessage = "Email, telefone ou CPF é obrigatório")]
        public string Credential { get; set; }
    }
}
