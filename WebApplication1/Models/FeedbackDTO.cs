using System;

namespace TechDesk.Models
{
    public class FeedbackDTO
    {
        public int Id { get; set; }
        public int IdChamado { get; set; }
        public int UsuarioId { get; set; }
        public byte Nota { get; set; }
        public string? Comentario { get; set; }
        public DateTime Data { get; set; }
        public string? NomeUsuario { get; set; }
    }
}