namespace ERP.Application.DTOs
{
    public class LoanAdvanceOutputDTO
    {
        public long LoanAdvanceId { get; set; }
        public long EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public long? AccountId { get; set; }  // Da transação financeira relacionada
        public string AccountName { get; set; }  // Nome da conta
        public decimal Amount { get; set; }
        public long Installments { get; set; }
        public string DiscountSource { get; set; }
        public DateTime StartDate { get; set; }
        public string? Description { get; set; }
        public bool IsApproved { get; set; }
        public decimal InstallmentsPaid { get; set; }
        public decimal RemainingAmount { get; set; }
        public bool IsFullyPaid { get; set; }
        public DateTime LoanDate { get; set; }
        public List<CostCenterDistributionDTO> CostCenterDistributions { get; set; }  // Da transação financeira relacionada
        public long CriadoPor { get; set; }
        public long? AtualizadoPor { get; set; }
        public DateTime CriadoEm { get; set; }
        public DateTime? AtualizadoEm { get; set; }
    }
}
