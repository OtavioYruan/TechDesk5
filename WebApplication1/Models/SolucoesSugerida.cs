using System;
using System.Collections.Generic;

namespace TechDesk.Models;

public partial class SolucoesSugerida
{
    public int Id { get; set; }

    public int IdChamado { get; set; }

    public string Titulo { get; set; } = null!;

    public string? PassoApasso { get; set; }

    public string? LinksUteis { get; set; }

    public decimal? Confianca { get; set; }

    public DateTime CriadaEm { get; set; }

    public virtual Chamado IdChamadoNavigation { get; set; } = null!;
}
