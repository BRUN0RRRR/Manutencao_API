using System;
using System.Collections.Generic;

namespace manutencao_api.Models;

public partial class OrdemServico
{
    public int Id { get; set; }

    public string Titulo { get; set; } = null!;

    public string Descricao { get; set; } = null!;

    public string Categoria { get; set; } = null!;

    public string? Status { get; set; }

    public string? Prioridade { get; set; }

    public DateTime? DataAbertura { get; set; }

    public DateTime? DataFechamento { get; set; }

    public DateTime? PrazoResolucao { get; set; }

    public int? EquipamentoId { get; set; }

    public int RequerenteId { get; set; }

    public int? TecnicoId { get; set; }

    public int? GrupoResponsavelId { get; set; }

    public virtual Equipamento? Equipamento { get; set; }

    public virtual Grupo? GrupoResponsavel { get; set; }

    public virtual ICollection<HistoricoO> HistoricoOs { get; set; } = new List<HistoricoO>();

    public virtual ICollection<OrdemServicoAnexo> OrdemServicoAnexos { get; set; } = new List<OrdemServicoAnexo>();

    public virtual Usuario Requerente { get; set; } = null!;

    public virtual Usuario? Tecnico { get; set; }
}
