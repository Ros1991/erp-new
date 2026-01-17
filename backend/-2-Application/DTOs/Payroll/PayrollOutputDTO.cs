namespace ERP.Application.DTOs
{
    public class PayrollOutputDTO
    {
        public long PayrollId { get; set; }
        public long CompanyId { get; set; }
        public DateTime PeriodStartDate { get; set; }
        public DateTime PeriodEndDate { get; set; }
        public decimal TotalGrossPay { get; set; }
        public decimal TotalDeductions { get; set; }
        public decimal TotalNetPay { get; set; }
        public long TotalInss { get; set; }
        public long TotalFgts { get; set; }
        public int? ThirteenthPercentage { get; set; }
        public string? ThirteenthTaxOption { get; set; }
        public bool IsClosed { get; set; }
        public DateTime? ClosedAt { get; set; }
        public long? ClosedBy { get; set; }
        public string? ClosedByName { get; set; }
        public string? Notes { get; set; }
        public string? Snapshot { get; set; }
        public int EmployeeCount { get; set; }
        public bool IsLastPayroll { get; set; }
        public long CriadoPor { get; set; }
        public string CriadoPorNome { get; set; }
        public long? AtualizadoPor { get; set; }
        public string? AtualizadoPorNome { get; set; }
        public DateTime CriadoEm { get; set; }
        public DateTime? AtualizadoEm { get; set; }
    }
}
