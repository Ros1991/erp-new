using System.ComponentModel.DataAnnotations;

namespace ERP.Application.DTOs
{
    public class ContractCostCenterInputDTO
    {
        [Required(ErrorMessage = "ContractId é obrigatório")]
        public long ContractId { get; set; }

        [Required(ErrorMessage = "CostCenterId é obrigatório")]
        public long CostCenterId { get; set; }

        [Required(ErrorMessage = "Percentual é obrigatório")]
        public decimal Percentage { get; set; }
    }
}
