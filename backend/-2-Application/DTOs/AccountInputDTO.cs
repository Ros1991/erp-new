using System.ComponentModel.DataAnnotations;

namespace CAU.Application.DTOs
{
    public class AccountInputDTO
    {
		[Required(ErrorMessage = "CompanyId é obrigatório")]
		public long CompanyId { get; set; }
	
		[Required(ErrorMessage = "Name é obrigatório")]
		[StringLength(255, ErrorMessage = "Name deve ter no máximo 255 caracteres")]
		public string Name { get; set; }
	
		[Required(ErrorMessage = "Type é obrigatório")]
		[StringLength(50, ErrorMessage = "Type deve ter no máximo 50 caracteres")]
		public string Type { get; set; }
	
		[Required(ErrorMessage = "InitialBalance é obrigatório")]
		public long InitialBalance { get; set; }
    }
}
