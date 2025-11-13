namespace ERP.Application.DTOs
{
    public class TaskEmployeeOutputDTO
    {
        public long TaskEmployeeId { get; set; }
        public long TaskId { get; set; }
        public long EmployeeId { get; set; }
        public string Status { get; set; }
        public long? EstimatedHours { get; set; }
        public long? ActualHours { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public long CriadoPor { get; set; }
        public long? AtualizadoPor { get; set; }
        public DateTime CriadoEm { get; set; }
        public DateTime? AtualizadoEm { get; set; }
    }
}
