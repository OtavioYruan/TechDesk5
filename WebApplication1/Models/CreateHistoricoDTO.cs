namespace TechDesk.Models
{
    public class CreateHistoricoDTO
    {
        public int IdChamado { get; set; }
        public string AutorTipo { get; set; } = null!;
        public int? AutorUsuarioId { get; set; }
        public int? AutorTecnicoId { get; set; }
        public string Mensagem { get; set; } = null!;
        public string Visibilidade { get; set; } = "Externo";
        public string? StatusAntes { get; set; }
        public string? StatusDepois { get; set; }
    }
}