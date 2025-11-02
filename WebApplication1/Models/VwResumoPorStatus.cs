using System.ComponentModel.DataAnnotations.Schema;

namespace TechDesk.Models
{
    [Table("vw_ResumoPorStatus", Schema = "TechDesk")]
    public class VwResumoPorStatus
    {
        public string? Status { get; set; }
        public int? Quantidade { get; set; }
    }
}