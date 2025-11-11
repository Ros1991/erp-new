using ERP.Domain.Models;

namespace ERP.Application.DTOs
{
    public class RoleOutputDTO
    {
        public long RoleId { get; set; }
        public long CompanyId { get; set; }
        public string Name { get; set; }
        public RolePermissions Permissions { get; set; }
        public long CriadoPor { get; set; }
        public long? AtualizadoPor { get; set; }
        public DateTime CriadoEm { get; set; }
        public DateTime? AtualizadoEm { get; set; }
    }
}
