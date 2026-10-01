using System;
using System.Collections.Generic;

namespace manutencao_api.Models;

public partial class Usuario
{
    public int Id { get; set; }

    public string Nome { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string SenhaHash { get; set; } = null!;

    public string? Telefone { get; set; }

    public string? Ramal { get; set; }

    public string? Cargo { get; set; }

    public bool Ativo { get; set; }

    public DateTime? DataCriacao { get; set; }

    public DateTime? UltimoLogin { get; set; }

    public int PerfilBaseId { get; set; }

    public int? SetorId { get; set; }

    public virtual ICollection<HistoricoO> HistoricoOs { get; set; } = new List<HistoricoO>();

    public virtual ICollection<OrdemServicoAnexo> OrdemServicoAnexos { get; set; } = new List<OrdemServicoAnexo>();

    public virtual ICollection<OrdemServico> OrdemServicoRequerentes { get; set; } = new List<OrdemServico>();

    public virtual ICollection<OrdemServico> OrdemServicoTecnicos { get; set; } = new List<OrdemServico>();

    public virtual Perfi PerfilBase { get; set; } = null!;

    public virtual Setore? Setor { get; set; }

    public virtual ICollection<Grupo> Grupos { get; set; } = new List<Grupo>();
}
