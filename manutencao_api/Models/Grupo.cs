using System;
using System.Collections.Generic;

namespace manutencao_api.Models;

public partial class Grupo
{
    public int Id { get; set; }

    public string Nome { get; set; } = null!;

    public string? Descricao { get; set; }

    public virtual ICollection<OrdemServico> OrdemServicos { get; set; } = new List<OrdemServico>();

    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
