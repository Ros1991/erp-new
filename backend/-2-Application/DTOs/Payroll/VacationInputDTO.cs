namespace ERP.Application.DTOs
{
    public class VacationInputDTO
    {
        public long PayrollEmployeeId { get; set; }
        public int VacationDays { get; set; } = 30;
        public DateTime VacationStartDate { get; set; }
        public bool IncludeTaxes { get; set; } = true;
        public bool AdvanceNextMonth { get; set; } = true;
        public string? Notes { get; set; }
    }
}
