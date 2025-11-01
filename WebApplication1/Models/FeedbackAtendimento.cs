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

        // TINYINT → byte no C#
        public byte Nota { get; set; }

        [StringLength(500)]
        public string? Comentario { get; set; }

        public DateTime Data { get; set; }

        // Relacionamentos (nomes iguais ao banco)
        public Chamado? IdChamadoNavigation { get; set; }
        public Usuario? Usuario { get; set; }
    }
}