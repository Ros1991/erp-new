using System.ComponentModel.DataAnnotations;

namespace ERP.Application.DTOs
{
    public class EmployeeAllowedLocationInputDTO
    {
        [Required(ErrorMessage = "EmployeeId é obrigatório")]
        public long EmployeeId { get; set; }

        [Required(ErrorMessage = "LocationId é obrigatório")]
        public long LocationId { get; set; }
    }
}
