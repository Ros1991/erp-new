using System.ComponentModel.DataAnnotations;

namespace ERP.Application.DTOs
{
    public class TaskEmployeeInputDTO
    {
        [Required(ErrorMessage = "TaskId é obrigatório")]
        public long TaskId { get; set; }

        [Required(ErrorMessage = "EmployeeId é obrigatório")]
        public long EmployeeId { get; set; }

        [Required(ErrorMessage = "Status é obrigatório")]
        [StringLength(50, ErrorMessage = "Status deve ter no máximo 50 caracteres")]
        public string Status { get; set; }

        public long? EstimatedHours { get; set; }

        public long? ActualHours { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }
    }
}
