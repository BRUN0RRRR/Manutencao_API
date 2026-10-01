using System;
using System.Collections.Generic;

namespace manutencao_api.Models;

public partial class Equipamento
{
    public int Id { get; set; }

    public string Nome { get; set; } = null!;

    public string? Patrimonio { get; set; }

    public string? Status { get; set; }

    public int SetorId { get; set; }

    public virtual ICollection<OrdemServico> OrdemServicos { get; set; } = new List<OrdemServico>();

    public virtual Setore Setor { get; set; } = null!;
}
