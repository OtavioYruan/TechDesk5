using System;
using System.Collections.Generic;

namespace TechDesk.Models
{
    public partial class HistoricoChamado
    {
        public int Id { get; set; }

        public int IdChamado { get; set; }

        public DateTime Data { get; set; }

        public string AutorTipo { get; set; } = null!;

        public int? AutorUsuarioId { get; set; }

        public int? AutorTecnicoId { get; set; }

        public string Mensagem { get; set; } = null!;

        public string Visibilidade { get; set; } = null!;

        public string? StatusAntes { get; set; }

        public string? StatusDepois { get; set; }

        public virtual ICollection<AnexosMensagem> AnexosMensagems { get; set; } = new List<AnexosMensagem>();

        public virtual Tecnico? AutorTecnico { get; set; }

        public virtual Usuario? AutorUsuario { get; set; }

        // 🔧 Tornamos a navegação opcional para não quebrar o POST
        public virtual Chamado? IdChamadoNavigation { get; set; } = null!;
    }
}