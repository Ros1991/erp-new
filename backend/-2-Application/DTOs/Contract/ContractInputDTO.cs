using System.ComponentModel.DataAnnotations;

namespace ERP.Application.DTOs
{
    public class ContractInputDTO
    {
        [Required(ErrorMessage = "EmployeeId é obrigatório")]
        public long EmployeeId { get; set; }

        [Required(ErrorMessage = "Tipo é obrigatório")]
        [StringLength(50, ErrorMessage = "Tipo deve ter no máximo 50 caracteres")]
        public string Type { get; set; }

        [Required(ErrorMessage = "Valor é obrigatório")]
        public long Value { get; set; } // Em centavos

        [Required(ErrorMessage = "IsPayroll é obrigatório")]
        public bool IsPayroll { get; set; }

        [Required(ErrorMessage = "HasInss é obrigatório")]
        public bool HasInss { get; set; }

        [Required(ErrorMessage = "HasIrrf é obrigatório")]
        public bool HasIrrf { get; set; }

        [Required(ErrorMessage = "HasFgts é obrigatório")]
        public bool HasFgts { get; set; }

        public bool HasThirteenthSalary { get; set; } = true;

        public bool HasVacationBonus { get; set; } = true;

        [Required(ErrorMessage = "Data de início é obrigatória")]
        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public long? WeeklyHours { get; set; }

        [Required(ErrorMessage = "Status ativo é obrigatório")]
        public bool IsActive { get; set; }

        public List<ContractBenefitDiscountDTO>? BenefitsDiscounts { get; set; }
        public List<ContractCostCenterDTO>? CostCenters { get; set; }
    }
}
