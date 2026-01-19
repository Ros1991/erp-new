using System.ComponentModel.DataAnnotations;

namespace ERP.Application.DTOs
{
    public class AccountPayableReceivableInputDTO
    {
        public long? SupplierCustomerId { get; set; }

        public long? AccountId { get; set; }

        [Required(ErrorMessage = "Descrição é obrigatória")]
        [StringLength(255, ErrorMessage = "Descrição deve ter no máximo 255 caracteres")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Tipo é obrigatório")]
        [StringLength(50, ErrorMessage = "Tipo deve ter no máximo 50 caracteres")]
        public string Type { get; set; }

        [Required(ErrorMessage = "Valor é obrigatório")]
        public long Amount { get; set; }

        [Required(ErrorMessage = "Data de vencimento é obrigatória")]
        public DateTime DueDate { get; set; }

        [Required(ErrorMessage = "Status de pagamento é obrigatório")]
        public bool IsPaid { get; set; }

        // Data do pagamento (usada quando IsPaid=true, opcional - se não informada, usa DueDate)
        public DateTime? PaymentDate { get; set; }

        // Lista de distribuição por centro de custo (opcional)
        public List<CostCenterDistributionDTO>? CostCenterDistributions { get; set; }
    }
}
