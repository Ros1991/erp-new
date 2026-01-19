namespace ERP.Application.DTOs
{
    public class FinancialTransactionOutputDTO
    {
        public long FinancialTransactionId { get; set; }
        public long CompanyId { get; set; }
        public long? AccountId { get; set; }
        public string AccountName { get; set; }
        public long? PurchaseOrderId { get; set; }
        public long? AccountPayableReceivableId { get; set; }
        public long? SupplierCustomerId { get; set; }
        public string SupplierCustomerName { get; set; }
        public long? LoanAdvanceId { get; set; }
        public long? PayrollId { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public long Amount { get; set; }
        public DateTime TransactionDate { get; set; }
        public List<CostCenterDistributionDTO>? CostCenterDistributions { get; set; }
        public long CriadoPor { get; set; }
        public long? AtualizadoPor { get; set; }
        public DateTime CriadoEm { get; set; }
        public DateTime? AtualizadoEm { get; set; }
    }
}
