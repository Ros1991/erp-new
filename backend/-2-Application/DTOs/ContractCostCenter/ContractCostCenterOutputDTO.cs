namespace ERP.Application.DTOs
{
    public class ContractCostCenterOutputDTO
    {
        public long ContractCostCenterId { get; set; }
        public long ContractId { get; set; }
        public long CostCenterId { get; set; }
        public long Percentage { get; set; }
        public long CriadoPor { get; set; }
        public long? AtualizadoPor { get; set; }
        public DateTime CriadoEm { get; set; }
        public DateTime? AtualizadoEm { get; set; }
    }
}
