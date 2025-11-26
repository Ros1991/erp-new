using System.ComponentModel.DataAnnotations;

namespace ERP.Application.DTOs
{
    public class PayrollInputDTO
    {
        [Required(ErrorMessage = "Data de início do período é obrigatória")]
        public DateTime PeriodStartDate { get; set; }

        [Required(ErrorMessage = "Data de fim do período é obrigatória")]
        public DateTime PeriodEndDate { get; set; }

        [StringLength(1000, ErrorMessage = "Observações devem ter no máximo 1000 caracteres")]
        public string? Notes { get; set; }
    }
}
