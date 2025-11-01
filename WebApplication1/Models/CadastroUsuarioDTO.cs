namespace TechDesk.Models.DTOs
{
    public class CadastroUsuarioDTO
    {
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string SenhaHash { get; set; } = string.Empty;
        public string? Perfil { get; set; }
    }
}
