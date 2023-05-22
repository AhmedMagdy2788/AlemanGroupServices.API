using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlemanGroupServices.Core.Models
{
    public class DigitalCountersReads
    {
        [Key]
        [Column(Order = 1)]
        public DateTime Registeration_Date { get; set; }
        [Key]
        [Column(Order = 2)]
        [ForeignKey("StationsCounter")]
        public uint Counter_No { get; set; }
        public double Counter_Reading { get; set; }
    }
}
