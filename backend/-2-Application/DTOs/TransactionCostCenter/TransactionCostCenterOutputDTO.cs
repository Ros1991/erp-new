namespace ERP.Application.DTOs
{
    public class TransactionCostCenterOutputDTO
    {
        public long TransactionCostCenterId { get; set; }
        public long FinancialTransactionId { get; set; }
        public long CostCenterId { get; set; }
        public long Amount { get; set; }
        public long CriadoPor { get; set; }
        public long? AtualizadoPor { get; set; }
        public DateTime CriadoEm { get; set; }
        public DateTime? AtualizadoEm { get; set; }
    }
}
