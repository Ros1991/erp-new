using ERP.Application.DTOs.Base;

namespace ERP.Application.DTOs
{
    /// <summary>
    /// Filtros específicos para User
    /// </summary>
    public class UserFilterDTO : PagedRequest
    {
        public string? SearchTerm { get; set; }
    }
}
