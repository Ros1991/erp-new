using ERP.Application.DTOs.Base;

namespace ERP.Application.DTOs
{
    /// <summary>
    /// Filtros específicos para Role
    /// </summary>
    public class RoleFilterDTO : PagedRequest
    {
        /// <summary>
        /// Filtro por nome do cargo
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Filtro por tipo de cargo (sistema ou customizado)
        /// </summary>
        public bool? IsSystem { get; set; }
    }
}
