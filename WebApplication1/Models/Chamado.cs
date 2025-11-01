using System;
using System.Collections.Generic;

namespace TechDesk.Models
{
    public partial class Chamado
    {
        public int IdChamado { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public string Prioridade { get; set; } = string.Empty;
        public string Status { get; set; } = "Aberto";
        public DateTime DataInicio { get; set; }
        public DateTime? DataFinal { get; set; }
        public string Nivel { get; set; } = "N1";
        public int IdUsuario { get; set; }
        public int IdCategoria { get; set; }
        public int? IdTecnico { get; set; }

        // 🔗 Relacionamentos (ligações com outras tabelas)
        public virtual Categoria IdCategoriaNavigation { get; set; } = null!;
        public virtual Tecnico? IdTecnicoNavigation { get; set; }
        public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
        public virtual ICollection<HistoricoChamado> HistoricoChamados { get; set; } = new List<HistoricoChamado>();
        public virtual ICollection<FeedbackAtendimento> FeedbackAtendimentos { get; set; } = new List<FeedbackAtendimento>();

        // ✅ Adicionando esta linha para corrigir o erro CS1061:
        public virtual ICollection<SolucoesSugerida> SolucoesSugerida { get; set; } = new List<SolucoesSugerida>();
    }
}