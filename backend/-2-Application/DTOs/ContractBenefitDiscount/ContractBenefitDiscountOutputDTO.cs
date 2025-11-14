namespace ERP.Application.DTOs
{
    public class ContractBenefitDiscountOutputDTO
    {
        public long ContractBenefitDiscountId { get; set; }
        public long ContractId { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public string Application { get; set; }
        public decimal Amount { get; set; }
        public long CriadoPor { get; set; }
        public long? AtualizadoPor { get; set; }
        public DateTime CriadoEm { get; set; }
        public DateTime? AtualizadoEm { get; set; }
    }
}
