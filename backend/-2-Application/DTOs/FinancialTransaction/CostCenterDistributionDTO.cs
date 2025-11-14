using System.ComponentModel.DataAnnotations;

namespace ERP.Application.DTOs
{
    public class CostCenterDistributionDTO
    {
        [Required(ErrorMessage = "Centro de custo é obrigatório")]
        public long CostCenterId { get; set; }

        public string? CostCenterName { get; set; }

        [Required(ErrorMessage = "Porcentagem é obrigatória")]
        [Range(0.01, 100, ErrorMessage = "Porcentagem deve estar entre 0.01 e 100")]
        public decimal Percentage { get; set; }

        public long? Amount { get; set; } // Calculado no backend
    }
}
