using System.ComponentModel.DataAnnotations;

namespace ERP.Application.DTOs
{
    // DTO simplificado para criação/edição de ordem de compra (qualquer funcionário)
    public class PurchaseOrderInputDTO
    {
        [Required(ErrorMessage = "Descrição é obrigatória")]
        [StringLength(500, ErrorMessage = "Descrição deve ter no máximo 500 caracteres")]
        public string Description { get; set; }
    }

    // DTO para processamento (aprovar/rejeitar) - apenas quem tem permissão canProcess
    public class PurchaseOrderProcessDTO
    {
        [Required(ErrorMessage = "Status é obrigatório")]
        public string Status { get; set; } // "Aprovado" ou "Rejeitado"

        [StringLength(500, ErrorMessage = "Mensagem deve ter no máximo 500 caracteres")]
        public string? Message { get; set; }

        // Data do processamento (informada pelo usuário)
        [Required(ErrorMessage = "Data do processamento é obrigatória")]
        public DateTime ProcessedAt { get; set; }

        // Campos obrigatórios apenas quando Status = "Aprovado"
        public long? TotalAmount { get; set; }
        public long? AccountId { get; set; }
        
        // Descrição da transação (sugestão: descrição da ordem, mas editável)
        public string? TransactionDescription { get; set; }
        
        public List<CostCenterDistributionInputDTO>? CostCenterDistributions { get; set; }
    }

    public class CostCenterDistributionInputDTO
    {
        public long CostCenterId { get; set; }
        public decimal Percentage { get; set; }
        public long Amount { get; set; }
    }
}
