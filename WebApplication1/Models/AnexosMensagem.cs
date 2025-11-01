using System;
using System.Collections.Generic;

namespace TechDesk.Models;

public partial class AnexosMensagem
{
    public int Id { get; set; }

    public int IdMensagem { get; set; }

    public string NomeArquivo { get; set; } = null!;

    public string? Descricao { get; set; }

    public string? Url { get; set; }

    public string? ContentType { get; set; }

    public long? TamanhoBytes { get; set; }

    public DateTime DataUpload { get; set; }

    public virtual HistoricoChamado IdMensagemNavigation { get; set; } = null!;
}
