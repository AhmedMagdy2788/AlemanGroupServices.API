using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AlemanGroupServices.Core.Models
{
    public partial class Tbltank
    {
        [Key]
        public uint Tank_No { get; set; }
        public string Tank_Name { get; set; } = null!;
        public int Station_id { get; set; }
        public int Max_Capacity { get; set; }
        //public virtual Tblstation StationNameNavigation { get; set; } = null!;
    }

    public class TankDTO
    {
        [Key]
        public uint Tank_No { get; set; }
        public string Tank_Name { get; set; } = null!;
        public string Station_Name { get; set; } = null!;
        public int Max_Capacity { get; set; }
    }

    public class TanksPairs
    {
        [Key]
        public uint Tank_No { get; set; }
        public string Tank_Name { get; set; } = null!;
    }
}
