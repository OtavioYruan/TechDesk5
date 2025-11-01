namespace TechDesk.Models
{
    public class CreateChamadoDTO
    {
        public string Titulo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public string Prioridade { get; set; } = string.Empty;
        public int IdUsuario { get; set; }
        public int IdCategoria { get; set; }
        public int? IdTecnico { get; set; }
        public string? Nivel { get; set; }
    }
}