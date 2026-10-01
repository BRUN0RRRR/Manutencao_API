using System;
using System.Collections.Generic;

namespace manutencao_api.Models;

public partial class Perfi
{
    public int Id { get; set; }

    public string Nome { get; set; } = null!;

    public int NivelAcesso { get; set; }

    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
