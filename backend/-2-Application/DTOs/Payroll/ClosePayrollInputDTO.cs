namespace ERP.Application.DTOs
{
    public class ClosePayrollInputDTO
    {
        public long AccountId { get; set; }
        public DateTime PaymentDate { get; set; }
        public long InssAmount { get; set; }
        public long FgtsAmount { get; set; }
    }
}
