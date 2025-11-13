using System.ComponentModel.DataAnnotations;

namespace ERP.Application.DTOs
{
    public class TaskInputDTO
    {
        public long? TaskIdParent { get; set; }

        public long? TaskIdBlocking { get; set; }

        [Required(ErrorMessage = "Título é obrigatório")]
        [StringLength(200, ErrorMessage = "Título deve ter no máximo 200 caracteres")]
        public string Title { get; set; }

        [StringLength(1000, ErrorMessage = "Descrição deve ter no máximo 1000 caracteres")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Prioridade é obrigatória")]
        [StringLength(50, ErrorMessage = "Prioridade deve ter no máximo 50 caracteres")]
        public string Priority { get; set; }

        public long? FrequencyDays { get; set; }

        [Required(ErrorMessage = "Permitir domingo é obrigatório")]
        public bool AllowSunday { get; set; }

        [Required(ErrorMessage = "Permitir segunda é obrigatório")]
        public bool AllowMonday { get; set; }

        [Required(ErrorMessage = "Permitir terça é obrigatório")]
        public bool AllowTuesday { get; set; }

        [Required(ErrorMessage = "Permitir quarta é obrigatório")]
        public bool AllowWednesday { get; set; }

        [Required(ErrorMessage = "Permitir quinta é obrigatório")]
        public bool AllowThursday { get; set; }

        [Required(ErrorMessage = "Permitir sexta é obrigatório")]
        public bool AllowFriday { get; set; }

        [Required(ErrorMessage = "Permitir sábado é obrigatório")]
        public bool AllowSaturday { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        [Required(ErrorMessage = "Status geral é obrigatório")]
        [StringLength(50, ErrorMessage = "Status geral deve ter no máximo 50 caracteres")]
        public string OverallStatus { get; set; }
    }
}
