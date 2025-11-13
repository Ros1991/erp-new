using System.ComponentModel.DataAnnotations;

namespace ERP.Application.DTOs
{
    public class LocationInputDTO
    {
        [Required(ErrorMessage = "Nome é obrigatório")]
        [StringLength(100, ErrorMessage = "Nome deve ter no máximo 100 caracteres")]
        public string Name { get; set; }

        [StringLength(255, ErrorMessage = "Endereço deve ter no máximo 255 caracteres")]
        public string? Address { get; set; }

        [Required(ErrorMessage = "Latitude é obrigatória")]
        public long Latitude { get; set; }

        [Required(ErrorMessage = "Longitude é obrigatória")]
        public long Longitude { get; set; }

        [Required(ErrorMessage = "Raio em metros é obrigatório")]
        public long RadiusMeters { get; set; }

        [Required(ErrorMessage = "Status ativo é obrigatório")]
        public bool IsActive { get; set; }
    }
}
