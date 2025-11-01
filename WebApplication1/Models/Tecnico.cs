using System;
using System.Collections.Generic;

namespace TechDesk.Models
{
    public partial class Tecnico
    {
        public int Id { get; set; }

        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string SenhaHash { get; set; } = string.Empty;
        public string Perfil { get; set; } = "Tecnico";
        public string? Especialidade { get; set; }
        public string? Nivel { get; set; }
        public string? CodigoEmpresa { get; set; }

        public bool Ativo { get; set; } = true;
        public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
        public DateTime? AtualizadoEm { get; set; }

        // 🔗 Relacionamentos (mantidos como no banco)
        public virtual ICollection<Chamado> Chamados { get; set; } = new List<Chamado>();
        public virtual ICollection<HistoricoChamado> HistoricoChamados { get; set; } = new List<HistoricoChamado>();
        public virtual ICollection<Categoria> Categoria { get; set; } = new List<Categoria>();
    }
}