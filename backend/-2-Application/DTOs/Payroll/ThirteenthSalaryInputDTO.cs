namespace ERP.Application.DTOs
{
    public class ThirteenthSalaryInputDTO
    {
        public int Percentage { get; set; } = 100;
        public string TaxOption { get; set; } = "none"; // "none", "proportional", "full"
    }
}
