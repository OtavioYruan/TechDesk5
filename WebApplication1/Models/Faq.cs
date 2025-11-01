using System;
using System.Collections.Generic;

namespace TechDesk.Models;

public partial class Faq
{
    public int Id { get; set; }

    public string Pergunta { get; set; } = null!;

    public string Resposta { get; set; } = null!;

    public int? CategoriaId { get; set; }

    public virtual Categoria? Categoria { get; set; }
}
