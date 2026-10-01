using System.ComponentModel.DataAnnotations;

namespace manutencao_api.Models.DTOs
{
    public abstract class SetorBaseDto
    {
        [Required(ErrorMessage = "O nome do setor é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome do setor não pode exceder 100 caracteres.")]
        public string Nome { get; set; } = null!;
    }

    // DTO de Leitura (GET)
    public class SetorDto : SetorBaseDto
    {
        public int Id { get; set; }
    }

    // DTO de Criação (POST)
    public class CreateSetorDto : SetorBaseDto
    {
    }

    // DTO de Atualização (PUT)
    public class UpdateSetorDto : SetorBaseDto
    {
        [Required]
        public int Id { get; set; }
    }
}