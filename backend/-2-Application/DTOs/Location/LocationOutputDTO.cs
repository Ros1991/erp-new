namespace ERP.Application.DTOs
{
    public class LocationOutputDTO
    {
        public long LocationId { get; set; }
        public long CompanyId { get; set; }
        public string Name { get; set; }
        public string? Address { get; set; }
        public long Latitude { get; set; }
        public long Longitude { get; set; }
        public long RadiusMeters { get; set; }
        public bool IsActive { get; set; }
        public long CriadoPor { get; set; }
        public long? AtualizadoPor { get; set; }
        public DateTime CriadoEm { get; set; }
        public DateTime? AtualizadoEm { get; set; }
    }
}
