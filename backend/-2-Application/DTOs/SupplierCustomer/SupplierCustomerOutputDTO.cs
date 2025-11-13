namespace ERP.Application.DTOs
{
    public class SupplierCustomerOutputDTO
    {
        public long SupplierCustomerId { get; set; }
        public long CompanyId { get; set; }
        public string Name { get; set; }
        public string? Document { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public bool IsActive { get; set; }
        public long CriadoPor { get; set; }
        public long? AtualizadoPor { get; set; }
        public DateTime CriadoEm { get; set; }
        public DateTime? AtualizadoEm { get; set; }
    }
}
