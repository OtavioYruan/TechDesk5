namespace TechDesk.Models
{
    public class LoginResponse
    {
        public int UsuarioId { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
    }
}
