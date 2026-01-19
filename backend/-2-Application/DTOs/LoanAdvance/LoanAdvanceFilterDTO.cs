
namespace ERP.Application.DTOs
{
    /// <summary>
    /// Filtros específicos para LoanAdvance
    /// </summary>
    public class LoanAdvanceFilterDTO : PagedRequest
    {
        /// <summary>
        /// Se true, retorna apenas empréstimos em aberto (não totalmente pagos)
        /// </summary>
        public bool? OnlyOpen { get; set; }
    }
}
