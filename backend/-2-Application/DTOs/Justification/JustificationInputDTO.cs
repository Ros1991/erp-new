using System.ComponentModel.DataAnnotations;

namespace ERP.Application.DTOs
{
    public class JustificationInputDTO
    {
        [Required(ErrorMessage = "EmployeeId é obrigatório")]
        public long EmployeeId { get; set; }

        [Required(ErrorMessage = "Data de referência é obrigatória")]
        public DateTime ReferenceDate { get; set; }

        [Required(ErrorMessage = "Motivo é obrigatório")]
        [StringLength(500, ErrorMessage = "Motivo deve ter no máximo 500 caracteres")]
        public string Reason { get; set; }

        [StringLength(2048, ErrorMessage = "URL do anexo deve ter no máximo 2048 caracteres")]
        public string? AttachmentUrl { get; set; }

        public long? HoursGranted { get; set; }

        public long? UserIdApprover { get; set; }

        [Required(ErrorMessage = "Status é obrigatório")]
        [StringLength(50, ErrorMessage = "Status deve ter no máximo 50 caracteres")]
        public string Status { get; set; }
    }
}
