namespace ERP.Application.DTOs
{
    public class AccountPayableReceivableOutputDTO
    {
        public long AccountPayableReceivableId { get; set; }
        public long CompanyId { get; set; }
        public long? SupplierCustomerId { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public long Amount { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsPaid { get; set; }
        public long CriadoPor { get; set; }
        public long? AtualizadoPor { get; set; }
        public DateTime CriadoEm { get; set; }
        public DateTime? AtualizadoEm { get; set; }
    }
}
