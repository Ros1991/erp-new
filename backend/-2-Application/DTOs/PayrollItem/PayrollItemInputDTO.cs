using System.ComponentModel.DataAnnotations;

namespace ERP.Application.DTOs
{
    public class PayrollItemInputDTO
    {
        [Required(ErrorMessage = "PayrollEmployeeId é obrigatório")]
        public long PayrollEmployeeId { get; set; }

        [Required(ErrorMessage = "Descrição é obrigatória")]
        [StringLength(255, ErrorMessage = "Descrição deve ter no máximo 255 caracteres")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Tipo é obrigatório")]
        [StringLength(50, ErrorMessage = "Tipo deve ter no máximo 50 caracteres")]
        public string Type { get; set; }

        [Required(ErrorMessage = "Categoria é obrigatória")]
        [StringLength(50, ErrorMessage = "Categoria deve ter no máximo 50 caracteres")]
        public string Category { get; set; }

        [Required(ErrorMessage = "Valor é obrigatório")]
        public long Amount { get; set; }

        public long? ReferenceId { get; set; }

        public long? CalculationBasis { get; set; }

        [StringLength(500, ErrorMessage = "Detalhes do cálculo deve ter no máximo 500 caracteres")]
        public string? CalculationDetails { get; set; }
    }
}
