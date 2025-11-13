namespace ERP.Application.DTOs
{
    public class PayrollOutputDTO
    {
        public long PayrollId { get; set; }
        public long CompanyId { get; set; }
        public DateTime PeriodStartDate { get; set; }
        public DateTime PeriodEndDate { get; set; }
        public long TotalGrossPay { get; set; }
        public long TotalDeductions { get; set; }
        public long TotalNetPay { get; set; }
        public bool IsClosed { get; set; }
        public long CriadoPor { get; set; }
        public long? AtualizadoPor { get; set; }
        public DateTime CriadoEm { get; set; }
        public DateTime? AtualizadoEm { get; set; }
    }
}
