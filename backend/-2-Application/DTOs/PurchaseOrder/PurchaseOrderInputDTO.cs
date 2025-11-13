using System.ComponentModel.DataAnnotations;

namespace ERP.Application.DTOs
{
    public class PurchaseOrderInputDTO
    {
        [Required(ErrorMessage = "UserIdRequester é obrigatório")]
        public long UserIdRequester { get; set; }

        public long? UserIdApprover { get; set; }

        [Required(ErrorMessage = "Descrição é obrigatória")]
        [StringLength(500, ErrorMessage = "Descrição deve ter no máximo 500 caracteres")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Valor total é obrigatório")]
        public long TotalAmount { get; set; }

        [Required(ErrorMessage = "Status é obrigatório")]
        [StringLength(50, ErrorMessage = "Status deve ter no máximo 50 caracteres")]
        public string Status { get; set; }
    }
}
