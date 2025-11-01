namespace TechDesk.Models
{
    public class UsuarioDTO
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Perfil { get; set; } = string.Empty;
        public bool Ativo { get; set; }
        public DateTime? CriadoEm { get; set; }   // ✅ Corrigido
        public DateTime? AtualizadoEm { get; set; } // ✅ Corrigido
    }
}