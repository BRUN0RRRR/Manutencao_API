namespace manutencao_api.Models.DTOs
{
    public class EquipamentoDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = null!;
        public string? Patrimonio { get; set; }
        public string? Status { get; set; }

        // Retornamos o ID do setor
        public int SetorId { get; set; }

        // Opcional: Se quiser retornar o nome do setor na consulta
        public string? NomeSetor { get; set; }
    }
}