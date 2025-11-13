namespace ERP.Application.DTOs
{
    public class EmployeeAllowedLocationOutputDTO
    {
        public long EmployeeAllowedLocationId { get; set; }
        public long EmployeeId { get; set; }
        public long LocationId { get; set; }
        public long CriadoPor { get; set; }
        public long? AtualizadoPor { get; set; }
        public DateTime CriadoEm { get; set; }
        public DateTime? AtualizadoEm { get; set; }
    }
}
