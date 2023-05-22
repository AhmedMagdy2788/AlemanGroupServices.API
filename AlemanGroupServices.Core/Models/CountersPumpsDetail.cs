using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AlemanGroupServices.Core.Models
{
    public partial class CountersPumpsDetail
    {
        [Key]
        [Column(Order = 1)]
        public DateTime Date { get; set; }
        [Key]
        [Column(Order = 2)]
        public uint PumpNo { get; set; }
        [Key]
        [Column(Order = 3)]
        public int NozelNo { get; set; }
        [Column(Order = 4)]
        [ForeignKey("StationsCounter")]
        public uint CounterNo { get; set; }
    }
}
