
namespace ERP.Application.DTOs
{
    /// <summary>
    /// Filtros específicos para Payroll
    /// </summary>
    public class PayrollFilterDTO : PagedRequest
    {
        /// <summary>
        /// Filtro por data de início do período
        /// </summary>
        public DateTime? PeriodStartDate { get; set; }

        /// <summary>
        /// Filtro por data de fim do período
        /// </summary>
        public DateTime? PeriodEndDate { get; set; }

        /// <summary>
        /// Filtro por status de fechamento
        /// </summary>
        public bool? IsClosed { get; set; }
    }
}
