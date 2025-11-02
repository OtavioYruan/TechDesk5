using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TechDesk.Models
{
    [Table("Feedback_Atendimento", Schema = "TechDesk")]
    public class FeedbackAtendimento
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Chamado")]
        public int IdChamado { get; set; }

        [ForeignKey("Usuario")]
        public int UsuarioId { get; set; }

        // tinyint no banco → byte no C#
        public byte Nota { get; set; }

        [StringLength(1000)]
        public string? Comentario { get; set; }

        public DateTime Data { get; set; }

        // 🔹 Nomes de navegação exatamente como o banco e o DbContext esperam:
        public Chamado? IdChamadoNavigation { get; set; }

        public Usuario? Usuario { get; set; }
    }
}