namespace ERP.Application.DTOs
{
    public class PurchaseOrderOutputDTO
    {
        public long PurchaseOrderId { get; set; }
        public long CompanyId { get; set; }
        public long UserIdRequester { get; set; }
        public string? RequesterName { get; set; }
        public long? UserIdApprover { get; set; }
        public string? ApproverName { get; set; }
        public string Description { get; set; }
        public long TotalAmount { get; set; }
        public string Status { get; set; }
        public string? ProcessedMessage { get; set; }
        public DateTime? ProcessedAt { get; set; }
        public long? AccountId { get; set; }
        public string? AccountName { get; set; }
        public long CriadoPor { get; set; }
        public long? AtualizadoPor { get; set; }
        public DateTime CriadoEm { get; set; }
        public DateTime? AtualizadoEm { get; set; }
    }
}
