namespace TechDesk.Models
{
    public class CadastroTecnicoDTO
    {
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string SenhaHash { get; set; } = string.Empty;
        public string Perfil { get; set; } = "Tecnico";
        public string Especialidade { get; set; } = string.Empty;
        public string Nivel { get; set; } = string.Empty;
        public string CodigoEmpresa { get; set; } = string.Empty;
        public bool Ativo { get; set; } = true;
    }
}
