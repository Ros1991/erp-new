using System.ComponentModel.DataAnnotations;

namespace ERP.Application.DTOs
{
    public class TransactionCostCenterInputDTO
    {
        [Required(ErrorMessage = "FinancialTransactionId é obrigatório")]
        public long FinancialTransactionId { get; set; }

        [Required(ErrorMessage = "CostCenterId é obrigatório")]
        public long CostCenterId { get; set; }

        [Required(ErrorMessage = "Valor é obrigatório")]
        public long Amount { get; set; }

        [Required(ErrorMessage = "Porcentagem é obrigatória")]
        [Range(0.01, 100, ErrorMessage = "Porcentagem deve estar entre 0.01 e 100")]
        public decimal Percentage { get; set; }
    }
}
