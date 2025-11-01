namespace TechDesk.Models
{
    public class HistoricoChamadoDTO
    {
        public int Id { get; set; }
        public int IdChamado { get; set; }
        public DateTime Data { get; set; }
        public string AutorTipo { get; set; } = string.Empty;
        public int? AutorUsuarioId { get; set; }
        public int? AutorTecnicoId { get; set; }
        public string Mensagem { get; set; } = string.Empty;
        public string Visibilidade { get; set; } = string.Empty;
        public string? StatusAntes { get; set; }
        public string? StatusDepois { get; set; }
        public string? NomeAutor { get; set; }
    }
}