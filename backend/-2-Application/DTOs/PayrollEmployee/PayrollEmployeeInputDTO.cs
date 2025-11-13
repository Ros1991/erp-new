using System.ComponentModel.DataAnnotations;

namespace ERP.Application.DTOs
{
    public class PayrollEmployeeInputDTO
    {
        [Required(ErrorMessage = "PayrollId é obrigatório")]
        public long PayrollId { get; set; }

        [Required(ErrorMessage = "EmployeeId é obrigatório")]
        public long EmployeeId { get; set; }

        [Required(ErrorMessage = "Status de férias é obrigatório")]
        public bool IsOnVacation { get; set; }

        public long? VacationDays { get; set; }

        public long? VacationAdvanceAmount { get; set; }

        [Required(ErrorMessage = "Total bruto é obrigatório")]
        public long TotalGrossPay { get; set; }

        [Required(ErrorMessage = "Total de deduções é obrigatório")]
        public long TotalDeductions { get; set; }

        [Required(ErrorMessage = "Total líquido é obrigatório")]
        public long TotalNetPay { get; set; }
    }
}
