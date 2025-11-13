using System.ComponentModel.DataAnnotations;

namespace ERP.Application.DTOs
{
    public class TaskStatusHistoryInputDTO
    {
        [Required(ErrorMessage = "TaskEmployeeId é obrigatório")]
        public long TaskEmployeeId { get; set; }

        [StringLength(50, ErrorMessage = "Status antigo deve ter no máximo 50 caracteres")]
        public string? OldStatus { get; set; }

        [Required(ErrorMessage = "Novo status é obrigatório")]
        [StringLength(50, ErrorMessage = "Novo status deve ter no máximo 50 caracteres")]
        public string NewStatus { get; set; }

        [StringLength(500, ErrorMessage = "Motivo da mudança deve ter no máximo 500 caracteres")]
        public string? ChangeReason { get; set; }
    }
}
