namespace TechDesk.Models
{
    public class CreateMensagemDTO
    {
        public string? AutorTipo { get; set; }
        public int? AutorUsuarioId { get; set; }
        public int? AutorTecnicoId { get; set; }
        public string Mensagem { get; set; } = string.Empty;
        public string? Visibilidade { get; set; }
        public string? StatusAntes { get; set; }
        public string? StatusDepois { get; set; }

        public string? NomeArquivo { get; set; }
        public string? Url { get; set; }
        public string? DescricaoAnexo { get; set; }
        public string? ContentType { get; set; }
        public long? TamanhoBytes { get; set; }
    }
}