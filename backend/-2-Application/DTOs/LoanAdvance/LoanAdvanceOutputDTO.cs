namespace ERP.Application.DTOs
{
    public class LoanAdvanceOutputDTO
    {
        public long LoanAdvanceId { get; set; }
        public long EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public long Amount { get; set; }
        public long Installments { get; set; }
        public string DiscountSource { get; set; }
        public DateTime StartDate { get; set; }
        public bool IsApproved { get; set; }
        public long CriadoPor { get; set; }
        public long? AtualizadoPor { get; set; }
        public DateTime CriadoEm { get; set; }
        public DateTime? AtualizadoEm { get; set; }
    }
}
