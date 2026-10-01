using System;
using System.ComponentModel.DataAnnotations;

namespace manutencao_api.Models.DTOs
{
    public abstract class UsuarioBaseDto
    {
        [Required(ErrorMessage = "O nome é obrigatório.")]
        [StringLength(150)]
        public string Nome { get; set; } = null!;

        [Required(ErrorMessage = "O E-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "Formato de e-mail inválido.")]
        public string Email { get; set; } = null!;

        public string? Telefone { get; set; }
        public string? Ramal { get; set; }
        public string? Cargo { get; set; }

        [Required(ErrorMessage = "O Perfil Base é obrigatório.")]
        public int PerfilBaseId { get; set; }

        public int? SetorId { get; set; }

        public bool Ativo { get; set; } = true;
    }

    // DTO de Leitura (GET) - Oculta a palavra-passe (SenhaHash)
    public class UsuarioDto : UsuarioBaseDto
    {
        public int Id { get; set; }
        public DateTime? DataCriacao { get; set; }
        public DateTime? UltimoLogin { get; set; }

        // Campos auxiliares para facilitar a apresentação no frontend
        public string? NomePerfil { get; set; }
        public string? NomeSetor { get; set; }
    }

    // DTO de Criação (POST) - Recebe a palavra-passe limpa para ser convertida em Hash
    public class CreateUsuarioDto : UsuarioBaseDto
    {
        [Required(ErrorMessage = "A palavra-passe é obrigatória.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "A palavra-passe deve ter pelo menos 6 caracteres.")]
        public string Senha { get; set; } = null!;
    }

    // DTO de Atualização (PUT)
    public class UpdateUsuarioDto : UsuarioBaseDto
    {
        [Required]
        public int Id { get; set; }

        // Opcional: Se for preenchida, o sistema gera um novo hash e atualiza a palavra-passe
        public string? NovaSenha { get; set; }
    }
}