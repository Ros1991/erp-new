namespace ERP.Application.DTOs
{
    public class PayrollItemOutputDTO
    {
        public long PayrollItemId { get; set; }
        public long PayrollEmployeeId { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public string Category { get; set; }
        public long Amount { get; set; }
        public long? ReferenceId { get; set; }
        public long? CalculationBasis { get; set; }
        public string? CalculationDetails { get; set; }
        public long CriadoPor { get; set; }
        public long? AtualizadoPor { get; set; }
        public DateTime CriadoEm { get; set; }
        public DateTime? AtualizadoEm { get; set; }
    }
}
