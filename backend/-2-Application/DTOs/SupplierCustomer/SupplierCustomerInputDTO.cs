using System.ComponentModel.DataAnnotations;

namespace ERP.Application.DTOs
{
    public class SupplierCustomerInputDTO
    {
        [Required(ErrorMessage = "Nome é obrigatório")]
        [StringLength(255, ErrorMessage = "Nome deve ter no máximo 255 caracteres")]
        public string Name { get; set; }

        [StringLength(14, ErrorMessage = "Documento deve ter no máximo 14 caracteres")]
        public string? Document { get; set; }

        [StringLength(255, ErrorMessage = "E-mail deve ter no máximo 255 caracteres")]
        public string? Email { get; set; }

        [StringLength(20, ErrorMessage = "Telefone deve ter no máximo 20 caracteres")]
        public string? Phone { get; set; }

        [StringLength(1000, ErrorMessage = "Endereço deve ter no máximo 1000 caracteres")]
        public string? Address { get; set; }

        [Required(ErrorMessage = "Status ativo é obrigatório")]
        public bool IsActive { get; set; }
    }
}
