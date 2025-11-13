namespace ERP.Application.DTOs
{
    public class TaskStatusHistoryOutputDTO
    {
        public long TaskStatusHistoryId { get; set; }
        public long TaskEmployeeId { get; set; }
        public string? OldStatus { get; set; }
        public string NewStatus { get; set; }
        public string? ChangeReason { get; set; }
        public long CriadoPor { get; set; }
        public long? AtualizadoPor { get; set; }
        public DateTime CriadoEm { get; set; }
        public DateTime? AtualizadoEm { get; set; }
    }
}
