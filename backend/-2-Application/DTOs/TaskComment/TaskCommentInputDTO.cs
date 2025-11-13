using System.ComponentModel.DataAnnotations;

namespace ERP.Application.DTOs
{
    public class TaskCommentInputDTO
    {
        [Required(ErrorMessage = "TaskId é obrigatório")]
        public long TaskId { get; set; }

        [Required(ErrorMessage = "UserId é obrigatório")]
        public long UserId { get; set; }

        [Required(ErrorMessage = "Comentário é obrigatório")]
        [StringLength(1000, ErrorMessage = "Comentário deve ter no máximo 1000 caracteres")]
        public string Comment { get; set; }

        [StringLength(2048, ErrorMessage = "URL do anexo deve ter no máximo 2048 caracteres")]
        public string? AttachmentUrl { get; set; }
    }
}
