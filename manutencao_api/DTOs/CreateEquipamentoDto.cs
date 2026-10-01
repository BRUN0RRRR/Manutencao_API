using System.ComponentModel.DataAnnotations;

namespace manutencao_api.Models.DTOs
{
    public class CreateEquipamentoDto
    {
        [Required(ErrorMessage = "O nome do equipamento é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome não pode ter mais de 100 caracteres.")]
        public string Nome { get; set; } = null!;

        public string? Patrimonio { get; set; }

        public string? Status { get; set; }

        [Required(ErrorMessage = "O Setor do equipamento é obrigatório.")]
        public int SetorId { get; set; }
    }
}