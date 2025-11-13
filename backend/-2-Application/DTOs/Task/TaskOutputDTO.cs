namespace ERP.Application.DTOs
{
    public class TaskOutputDTO
    {
        public long TaskId { get; set; }
        public long CompanyId { get; set; }
        public long? TaskIdParent { get; set; }
        public long? TaskIdBlocking { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public string Priority { get; set; }
        public long? FrequencyDays { get; set; }
        public bool AllowSunday { get; set; }
        public bool AllowMonday { get; set; }
        public bool AllowTuesday { get; set; }
        public bool AllowWednesday { get; set; }
        public bool AllowThursday { get; set; }
        public bool AllowFriday { get; set; }
        public bool AllowSaturday { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string OverallStatus { get; set; }
        public long CriadoPor { get; set; }
        public long? AtualizadoPor { get; set; }
        public DateTime CriadoEm { get; set; }
        public DateTime? AtualizadoEm { get; set; }
    }
}
