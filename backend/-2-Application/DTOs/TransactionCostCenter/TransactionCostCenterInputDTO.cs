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
    }
}
