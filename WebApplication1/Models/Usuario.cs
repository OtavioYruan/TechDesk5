using System;
using System.Collections.Generic;

namespace TechDesk.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string SenhaHash { get; set; } = string.Empty;
        public string Perfil { get; set; } = "Usuario";
        public bool Ativo { get; set; } = true;
        public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
        public DateTime? AtualizadoEm { get; set; }

        // 🔗 Relacionamentos (necessários pro DbContext)
        public ICollection<Chamado>? Chamados { get; set; }
        public ICollection<FeedbackAtendimento>? FeedbackAtendimentos { get; set; }
        public ICollection<HistoricoChamado>? HistoricoChamados { get; set; }
        public PreferenciasNotificacao? PreferenciasNotificacao { get; set; }
    }
}
