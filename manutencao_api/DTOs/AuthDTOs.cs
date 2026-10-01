namespace manutencao_api.DTOs
{
    public class LoginDTO
    {
        public string Email { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
    }

    public class RegistroDTO
    {
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;

        // Opcional no momento do cadastro inicial
        public int? SetorId { get; set; }

        // Qual perfil o usuário terá? (Você pode forçar via código ou deixar a tela enviar)
        public int PerfilBaseId { get; set; }
    }
}