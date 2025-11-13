namespace ERP.Application.DTOs
{
    public class TaskCommentOutputDTO
    {
        public long TaskCommentId { get; set; }
        public long TaskId { get; set; }
        public long UserId { get; set; }
        public string Comment { get; set; }
        public string? AttachmentUrl { get; set; }
        public long CriadoPor { get; set; }
        public long? AtualizadoPor { get; set; }
        public DateTime CriadoEm { get; set; }
        public DateTime? AtualizadoEm { get; set; }
    }
}
