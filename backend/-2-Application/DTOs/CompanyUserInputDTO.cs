using System.ComponentModel.DataAnnotations;

namespace ERP.Application.DTOs
{
    public class CompanyUserInputDTO
    {
        [Required(ErrorMessage = "UserId é obrigatório")]
        public long UserId { get; set; }

        [Required(ErrorMessage = "RoleId é obrigatório")]
        public long RoleId { get; set; }
    }
}
