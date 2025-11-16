using System.ComponentModel.DataAnnotations;

namespace ERP.Application.DTOs
{
    public class AssociateUserDTO
    {
        [Required(ErrorMessage = "ID do usuário é obrigatório")]
        public long UserId { get; set; }
        
        public long? RoleId { get; set; }
        
        public bool CreateCompanyUser { get; set; }
    }
}
