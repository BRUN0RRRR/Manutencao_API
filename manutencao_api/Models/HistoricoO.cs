using System;
using System.Collections.Generic;

namespace manutencao_api.Models;

public partial class HistoricoO
{
    public int Id { get; set; }

    public string Comentario { get; set; } = null!;

    public DateTime? DataRegistro { get; set; }

    public int OrdemServicoId { get; set; }

    public int UsuarioId { get; set; }

    public virtual OrdemServico OrdemServico { get; set; } = null!;

    public virtual Usuario Usuario { get; set; } = null!;
}
