using System.ComponentModel.DataAnnotations;

namespace AlemanGroupServices.Core.Models
{
    public partial class StationsPumps
    {
        [Key]
        public uint PumpNo { get; set; }
        public string PumpName { get; set; } = null!;
    }
}
