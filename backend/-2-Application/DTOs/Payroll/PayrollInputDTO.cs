using System.ComponentModel.DataAnnotations;

namespace ERP.Application.DTOs
{
    public class PayrollInputDTO
    {
        [Required(ErrorMessage = "Data de início do período é obrigatória")]
        public DateTime PeriodStartDate { get; set; }

        [Required(ErrorMessage = "Data de fim do período é obrigatória")]
        public DateTime PeriodEndDate { get; set; }

        [Required(ErrorMessage = "Total bruto é obrigatório")]
        public long TotalGrossPay { get; set; }

        [Required(ErrorMessage = "Total de deduções é obrigatório")]
        public long TotalDeductions { get; set; }

        [Required(ErrorMessage = "Total líquido é obrigatório")]
        public long TotalNetPay { get; set; }

        [Required(ErrorMessage = "Status de fechamento é obrigatório")]
        public bool IsClosed { get; set; }
    }
}
