using System.ComponentModel.DataAnnotations;

namespace ERP.Application.DTOs
{
    public class ContractBenefitDiscountInputDTO
    {
        [Required(ErrorMessage = "ContractId é obrigatório")]
        public long ContractId { get; set; }

        [Required(ErrorMessage = "Descrição é obrigatória")]
        [StringLength(255, ErrorMessage = "Descrição deve ter no máximo 255 caracteres")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Tipo é obrigatório")]
        [StringLength(50, ErrorMessage = "Tipo deve ter no máximo 50 caracteres")]
        public string Type { get; set; }

        [Required(ErrorMessage = "Aplicação é obrigatória")]
        [StringLength(50, ErrorMessage = "Aplicação deve ter no máximo 50 caracteres")]
        public string Application { get; set; }

        [Required(ErrorMessage = "Valor é obrigatório")]
        public decimal Amount { get; set; }
    }
}
