using System.ComponentModel.DataAnnotations;

namespace ERP.Application.DTOs
{
    public class FinancialTransactionInputDTO
    {
        public long? AccountId { get; set; }

        public long? PurchaseOrderId { get; set; }

        public long? AccountPayableReceivableId { get; set; }

        public long? SupplierCustomerId { get; set; }

        [Required(ErrorMessage = "Descrição é obrigatória")]
        [StringLength(255, ErrorMessage = "Descrição deve ter no máximo 255 caracteres")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Tipo é obrigatório")]
        [StringLength(50, ErrorMessage = "Tipo deve ter no máximo 50 caracteres")]
        public string Type { get; set; }

        [Required(ErrorMessage = "Valor é obrigatório")]
        public long Amount { get; set; }

        [Required(ErrorMessage = "Data da transação é obrigatória")]
        public DateTime TransactionDate { get; set; }

        // Lista de distribuição por centro de custo (opcional)
        public List<CostCenterDistributionDTO>? CostCenterDistributions { get; set; }
    }
}
