namespace ERP.Application.DTOs
{
    public class TimeEntryOutputDTO
    {
        public long TimeEntryId { get; set; }
        public long EmployeeId { get; set; }
        public string Type { get; set; }
        public DateTime Timestamp { get; set; }
        public long? Latitude { get; set; }
        public long? Longitude { get; set; }
        public long? LocationId { get; set; }
        public long CriadoPor { get; set; }
        public long? AtualizadoPor { get; set; }
        public DateTime CriadoEm { get; set; }
        public DateTime? AtualizadoEm { get; set; }
    }
}
