namespace TechDesk.Models
{
    public class PreferenciasNotificacaoDTO
    {
        public int UsuarioId { get; set; }
        public bool Email { get; set; }
        public bool Push { get; set; }
        public bool StatusUpdates { get; set; }
    }
}