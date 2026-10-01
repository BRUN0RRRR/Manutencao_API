using System.ComponentModel.DataAnnotations;

namespace manutencao_api.Models.DTOs
{
    public abstract class PerfilBaseDto
    {
        [Required(ErrorMessage = "O nome do perfil é obrigatório.")]
        [StringLength(50, ErrorMessage = "O nome do perfil não pode exceder 50 caracteres.")]
        public string Nome { get; set; } = null!;

        [Required(ErrorMessage = "O Nível de Acesso é obrigatório.")]
        [Range(1, 999, ErrorMessage = "Informe um nível de acesso válido (ex: 1 para Admin, 2 para Técnico, etc.).")]
        public int NivelAcesso { get; set; }
    }

    // DTO de Leitura (GET)
    public class PerfilDto : PerfilBaseDto
    {
        public int Id { get; set; }
    }

    // DTO de Criação (POST)
    public class CreatePerfilDto : PerfilBaseDto
    {
    }

    // DTO de Atualização (PUT)
    public class UpdatePerfilDto : PerfilBaseDto
    {
        [Required]
        public int Id { get; set; }
    }
}