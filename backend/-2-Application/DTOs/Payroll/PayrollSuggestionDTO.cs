namespace ERP.Application.DTOs
{
    public class PayrollSuggestionDTO
    {
        public int SuggestedMonth { get; set; }
        public int SuggestedYear { get; set; }
        public bool HasOpenPayroll { get; set; }
        public long? OpenPayrollId { get; set; }
        public string? OpenPayrollPeriod { get; set; }
    }
}
