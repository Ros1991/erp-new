using System.ComponentModel.DataAnnotations;
using ERP.Domain.Models;

namespace ERP.Application.DTOs
{
    public class RoleInputDTO
    {
        [Required(ErrorMessage = "Name é obrigatório")]
        [StringLength(100, ErrorMessage = "Name deve ter no máximo 100 caracteres")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Permissions é obrigatório")]
        public RolePermissions Permissions { get; set; }
    }
}
