namespace TechDesk.Models
{
    public class CreateFeedbackDTO
    {
        public int UsuarioId { get; set; }
        public int Nota { get; set; }
        public string Comentario { get; set; } = string.Empty;
    }

    public class FeedbackDTO
    {
        public int Id { get; set; }
        public int IdChamado { get; set; }
        public int UsuarioId { get; set; }
        public int Nota { get; set; }
        public string Comentario { get; set; } = string.Empty;
        public DateTime Data { get; set; }
        public string? NomeUsuario { get; set; }
    }
}