namespace TechDesk.Models
{
    public class ChamadoListDTO
    {
        public int IdChamado { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public string Prioridade { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Nivel { get; set; } = string.Empty;
        public DateTime DataInicio { get; set; }
        public DateTime? DataFinal { get; set; }

        public string? UsuarioNome { get; set; }
        public string? CategoriaNome { get; set; }
        public string? TecnicoNome { get; set; }
    }
}