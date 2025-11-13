using System.ComponentModel.DataAnnotations;

namespace ERP.Application.DTOs
{
    public class UserInputDTO
    {
		[StringLength(255, ErrorMessage = "Email deve ter no máximo 255 caracteres")]
		public string? Email { get; set; }
	
		[StringLength(20, ErrorMessage = "Phone deve ter no máximo 20 caracteres")]
		public string? Phone { get; set; }
	
		[StringLength(11, ErrorMessage = "Cpf deve ter no máximo 11 caracteres")]
		public string? Cpf { get; set; }
	
		[Required(ErrorMessage = "Senha é obrigatória")]
		[MinLength(6, ErrorMessage = "Senha deve ter no mínimo 6 caracteres")]
		public string Password { get; set; }
    }
}
