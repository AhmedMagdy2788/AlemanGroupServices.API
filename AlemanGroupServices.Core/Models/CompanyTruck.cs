using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AlemanGroupServices.Core.Models
{
    public partial class CompanyTruck
    {
        [Key]
        public uint Id { get; set; }
        public string CompanyTruckNo { get; set; } = null!;
        public string Model { get; set; } = null!;
        public int? TruckLoad { get; set; } = null;
    }
}
