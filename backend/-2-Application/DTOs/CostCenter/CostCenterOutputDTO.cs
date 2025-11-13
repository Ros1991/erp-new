namespace ERP.Application.DTOs
{
    public class CostCenterOutputDTO
    {
        public long CostCenterId { get; set; }
        public long CompanyId { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public long CriadoPor { get; set; }
        public long? AtualizadoPor { get; set; }
        public DateTime CriadoEm { get; set; }
        public DateTime? AtualizadoEm { get; set; }
    }
}
