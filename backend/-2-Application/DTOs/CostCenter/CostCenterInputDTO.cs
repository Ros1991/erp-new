using System.ComponentModel.DataAnnotations;

namespace ERP.Application.DTOs
{
    public class CostCenterInputDTO
    {
        [Required(ErrorMessage = "Nome é obrigatório")]
        [StringLength(100, ErrorMessage = "Nome deve ter no máximo 100 caracteres")]
        public string Name { get; set; }

        [StringLength(255, ErrorMessage = "Descrição deve ter no máximo 255 caracteres")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Status ativo é obrigatório")]
        public bool IsActive { get; set; }
    }
}
