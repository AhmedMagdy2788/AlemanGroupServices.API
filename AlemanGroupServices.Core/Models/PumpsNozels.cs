using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlemanGroupServices.Core.Models
{
    public class PumpsNozels
    {
        [Key]
        [ForeignKey("StationsPumps")]
        public uint PumpNo { get; set; }
        [Key]
        public int NozelNo { get; set; }
    }
}
