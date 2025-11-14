using System.ComponentModel.DataAnnotations;

namespace ERP.Application.DTOs
{
    public class LoanAdvanceInputDTO
    {
        [Required(ErrorMessage = "EmployeeId é obrigatório")]
        public long EmployeeId { get; set; }

        [Required(ErrorMessage = "Valor é obrigatório")]
        public long Amount { get; set; }

        [Required(ErrorMessage = "Parcelas é obrigatório")]
        public long Installments { get; set; }

        [Required(ErrorMessage = "Fonte de desconto é obrigatória")]
        [StringLength(50, ErrorMessage = "Fonte de desconto deve ter no máximo 50 caracteres")]
        public string DiscountSource { get; set; }

        [Required(ErrorMessage = "Data de início é obrigatória")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "Status de aprovação é obrigatório")]
        public bool IsApproved { get; set; }

        public long? AccountId { get; set; }

        // Lista de distribuição por centro de custo (opcional)
        public List<CostCenterDistributionDTO>? CostCenterDistributions { get; set; }
    }
}
