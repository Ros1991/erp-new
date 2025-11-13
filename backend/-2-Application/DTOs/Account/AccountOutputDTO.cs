namespace ERP.Application.DTOs
{
    public class AccountOutputDTO
    {
		public long AccountId { get; set; }
		public long CompanyId { get; set; }
		public string Name { get; set; }
		public string Type { get; set; }
		public long InitialBalance { get; set; }
		public long CriadoPor { get; set; }
		public long? AtualizadoPor { get; set; }
		public DateTime CriadoEm { get; set; }
		public DateTime? AtualizadoEm { get; set; }
    }
}
