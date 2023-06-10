using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AlemanGroupServices.Core.Models
{
    public partial class PumpsTanksDetail
    {
        [Key]
        [Column(Order = 1)]
        public DateTime Date { get; set; }
        [Key]
        [Column(Order = 2)]
        [ForeignKey("StationsPumps")]
        public uint Pump_No { get; set; }
        [Column(Order = 3)]
        [ForeignKey("Tbltank")]
        public uint Tank_No { get; set; }

        //public virtual StationsPumps PumpNoNavigation { get; set; } = null!;
        //public virtual Tbltank TankNoNavigation { get; set; } = null!;
    }

    public class PumpTankDetailDto
    {
        public DateTime Date {get; set;}
        public uint Pump_No {get; set;}
        public string pump_Name { get; set; } = null!;
        public uint Tank_No { get; set;}
        public string Tank_Name { get; set; } = null!;
        public string tankContentType { get; set; } = null!;
    }

    public partial class PumpsCountersDailyReportDto
    {
        public PumpsTanksDetail? PumpInstallationDetails { get; set; }
        public Tbltankscontentstype? TankContentType { get; set; }
    }
}
