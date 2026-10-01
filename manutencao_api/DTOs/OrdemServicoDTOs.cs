namespace manutencao_api.DTOs
{
    public class NovaOsDTO
    {
        public string Titulo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public string Categoria { get; set; } = "Geral";
        public string Prioridade { get; set; } = "Normal"; // Baixa, Normal, Alta, Urgente

        // Opcionais: Pode ser para um equipamento específico e já ir para uma fila (grupo)
        public int? EquipamentoId { get; set; }
        public int? GrupoResponsavelId { get; set; }
        // Quando não houver autenticação, o cliente pode informar o requerente
        public int? RequerenteId { get; set; }
    }
}