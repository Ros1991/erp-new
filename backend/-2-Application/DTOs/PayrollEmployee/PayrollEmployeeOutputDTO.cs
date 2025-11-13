namespace ERP.Application.DTOs
{
    public class PayrollEmployeeOutputDTO
    {
        public long PayrollEmployeeId { get; set; }
        public long PayrollId { get; set; }
        public long EmployeeId { get; set; }
        public bool IsOnVacation { get; set; }
        public long? VacationDays { get; set; }
        public long? VacationAdvanceAmount { get; set; }
        public long TotalGrossPay { get; set; }
        public long TotalDeductions { get; set; }
        public long TotalNetPay { get; set; }
        public long CriadoPor { get; set; }
        public long? AtualizadoPor { get; set; }
        public DateTime CriadoEm { get; set; }
        public DateTime? AtualizadoEm { get; set; }
    }
}
