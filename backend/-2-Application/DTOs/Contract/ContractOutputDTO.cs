namespace ERP.Application.DTOs
{
    public class ContractOutputDTO
    {
        public long ContractId { get; set; }
        public long EmployeeId { get; set; }
        public string? EmployeeName { get; set; }
        public string Type { get; set; }
        public long Value { get; set; } // Em centavos
        public bool IsPayroll { get; set; }
        public bool HasInss { get; set; }
        public bool HasIrrf { get; set; }
        public bool HasFgts { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public long? WeeklyHours { get; set; }
        public bool IsActive { get; set; }
        public List<ContractBenefitDiscountDTO>? BenefitsDiscounts { get; set; }
        public List<ContractCostCenterDTO>? CostCenters { get; set; }
        public long CriadoPor { get; set; }
        public long? AtualizadoPor { get; set; }
        public DateTime CriadoEm { get; set; }
        public DateTime? AtualizadoEm { get; set; }
    }

    public class ContractBenefitDiscountDTO
    {
        public long? ContractBenefitDiscountId { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public string Application { get; set; }
        public long Amount { get; set; } // Em centavos
    }

    public class ContractCostCenterDTO
    {
        public long? ContractCostCenterId { get; set; }
        public long CostCenterId { get; set; }
        public string? CostCenterName { get; set; }
        public decimal Percentage { get; set; }
    }
}
