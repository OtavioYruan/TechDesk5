using System;
using System.Collections.Generic;

namespace TechDesk.Models;

public partial class VwResumoPorStatus
{
    public string Status { get; set; } = null!;

    public int? Qtde { get; set; }
}
