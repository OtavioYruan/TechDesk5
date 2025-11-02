using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace TechDesk.Models
{
    [Table("vw_TMA", Schema = "TechDesk")]
    public class VwTma
    {
        public double? TempoMedioMin { get; set; }
        public int? TotalChamados { get; set; }
        public DateTime? Data { get; set; }
    }
}