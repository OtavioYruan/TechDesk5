namespace TechDesk.Models
{
    public class CreateUsuarioDTO
    {
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string SenhaHash { get; set; } = string.Empty;
        public string Perfil { get; set; } = "Usuario";
        public bool Ativo { get; set; } = true;
    }
}