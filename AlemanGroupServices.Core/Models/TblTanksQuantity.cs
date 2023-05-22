using AlemanGroupServices.Core.Const;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlemanGroupServices.Core.Models
{
    public partial class TblTanksQuantity
    {
        [Key]
        [Column(Order = 0)]
        public DateTime Registeration_Date { get; set; }

        [Key]
        [Column(Order = 1)]
        public uint Tank_No { get; set; }

        public double Tank_Stock_Measurement { get; set; }

        //public virtual Tblpumptype ProductNavigation { get; set; } = null!;
        //public virtual Tbltank TankNoNavigation { get; set; } = null!;
    }
}
