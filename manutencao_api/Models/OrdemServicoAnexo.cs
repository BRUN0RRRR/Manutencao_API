using System;
using System.Collections.Generic;

namespace manutencao_api.Models;

public partial class OrdemServicoAnexo
{
    public int Id { get; set; }

    public string NomeOriginalArquivo { get; set; } = null!;

    public string CaminhoArquivo { get; set; } = null!;

    public string TipoArquivo { get; set; } = null!;

    public int TamanhoBytes { get; set; }

    public DateTime? DataUpload { get; set; }

    public int OrdemServicoId { get; set; }

    public int UploaderId { get; set; }

    public virtual OrdemServico OrdemServico { get; set; } = null!;

    public virtual Usuario Uploader { get; set; } = null!;
}
