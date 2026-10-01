// DTOs/SetorDTOs.cs
using System.ComponentModel.DataAnnotations;

namespace manutencao_api.DTOs
{
    public class CriarSetorDTO
    {
        [Required(ErrorMessage = "O nome do setor é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres.")]
        public string Nome { get; set; } = null!;
    }

    public class AtualizarSetorDTO
    {
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome do setor é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres.")]
        public string Nome { get; set; } = null!;
    }
}