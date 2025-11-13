namespace ERP.Application.DTOs
{
    public class JustificationOutputDTO
    {
        public long JustificationId { get; set; }
        public long EmployeeId { get; set; }
        public DateTime ReferenceDate { get; set; }
        public string Reason { get; set; }
        public string? AttachmentUrl { get; set; }
        public long? HoursGranted { get; set; }
        public long? UserIdApprover { get; set; }
        public string Status { get; set; }
        public long CriadoPor { get; set; }
        public long? AtualizadoPor { get; set; }
        public DateTime CriadoEm { get; set; }
        public DateTime? AtualizadoEm { get; set; }
    }
}
