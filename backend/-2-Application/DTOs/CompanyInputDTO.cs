using System.ComponentModel.DataAnnotations;

namespace ERP.Application.DTOs
{
    public class CompanyInputDTO
    {
		[Required(ErrorMessage = "Name é obrigatório")]
		[StringLength(255, ErrorMessage = "Name deve ter no máximo 255 caracteres")]
		public string Name { get; set; }
	
		[Required(ErrorMessage = "Document é obrigatório")]
		[StringLength(14, ErrorMessage = "Document deve ter no máximo 14 caracteres")]
		public string Document { get; set; }
	
		[Required(ErrorMessage = "UserId é obrigatório")]
		public long UserId { get; set; }
    }
}
