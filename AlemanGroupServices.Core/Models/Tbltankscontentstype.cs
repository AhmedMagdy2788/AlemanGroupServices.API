using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AlemanGroupServices.Core.Models
{
    public partial class Tbltankscontentstype
    {
        [Key]
        [Column(Order = 0)]
        public DateTime Date { get; set; }

        [Key]
        [Column(Order = 1)]
        public uint Tank_No { get; set; }

        public string Product { get; set; } = null!;

        //public virtual Tblpumptype ProductNavigation { get; set; } = null!;
        //public virtual Tbltank TankNoNavigation { get; set; } = null!;
    }

    public partial class TankscontentstypeDTO
    {
        public DateTime Date { get; set; }
        public string Tank_Name { get; set; } = null!;
        public string Product { get; set; } = null!;
    }
}
