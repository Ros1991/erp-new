using System.ComponentModel.DataAnnotations;

namespace ERP.Application.DTOs
{
    public class TimeEntryInputDTO
    {
        [Required(ErrorMessage = "EmployeeId é obrigatório")]
        public long EmployeeId { get; set; }

        [Required(ErrorMessage = "Tipo é obrigatório")]
        [StringLength(50, ErrorMessage = "Tipo deve ter no máximo 50 caracteres")]
        public string Type { get; set; }

        [Required(ErrorMessage = "Timestamp é obrigatório")]
        public DateTime Timestamp { get; set; }

        public long? Latitude { get; set; }

        public long? Longitude { get; set; }

        public long? LocationId { get; set; }
    }
}
