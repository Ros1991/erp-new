namespace ERP.Application.DTOs
{
    public class ContractOutputDTO
    {
        public long ContractId { get; set; }
        public long EmployeeId { get; set; }
        public string Type { get; set; }
        public long Value { get; set; }
        public bool IsPayroll { get; set; }
        public bool HasInss { get; set; }
        public bool HasIrrf { get; set; }
        public bool HasFgts { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public long? WeeklyHours { get; set; }
        public bool IsActive { get; set; }
        public long CriadoPor { get; set; }
        public long? AtualizadoPor { get; set; }
        public DateTime CriadoEm { get; set; }
        public DateTime? AtualizadoEm { get; set; }
    }
}
