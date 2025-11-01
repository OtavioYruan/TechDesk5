using System;
using System.Collections.Generic;

namespace TechDesk.Models;

public partial class PreferenciasNotificacao
{
    public int Id { get; set; }

    public int UsuarioId { get; set; }

    public bool Email { get; set; }

    public bool Push { get; set; }

    public bool StatusUpdates { get; set; }

    public DateTime AtualizadoEm { get; set; }

    public virtual Usuario Usuario { get; set; } = null!;
}
