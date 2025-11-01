namespace TechDesk.Models
{
    public class UpdateChamadoDTO
    {
        public string Titulo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public string Prioridade { get; set; } = string.Empty;
        public string Status { get; set; } = "Aberto";
        public int IdUsuario { get; set; }
        public int IdCategoria { get; set; }
        public int? IdTecnico { get; set; }
        public string Nivel { get; set; } = "N1";
        public DateTime? DataFinal { get; set; }
    }
}