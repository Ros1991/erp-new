namespace ERP.Application.DTOs
{
    public class CompanyOutputDTO
    {
		public long CompanyId { get; set; }
		public string Name { get; set; }
		public string Document { get; set; }
		public long UserId { get; set; }
		public long CriadoPor { get; set; }
		public long? AtualizadoPor { get; set; }
		public DateTime CriadoEm { get; set; }
		public DateTime? AtualizadoEm { get; set; }
    }
}
