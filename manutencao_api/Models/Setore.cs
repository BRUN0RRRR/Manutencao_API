using System;
using System.Collections.Generic;

namespace manutencao_api.Models;

public partial class Setore
{
    public int Id { get; set; }

    public string Nome { get; set; } = null!;

    public virtual ICollection<Equipamento> Equipamentos { get; set; } = new List<Equipamento>();

    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
