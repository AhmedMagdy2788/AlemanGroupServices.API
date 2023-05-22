using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AlemanGroupServices.Core.Models
{
    public partial class StationsCounter
    {
        [Key]
        public uint CounterNo { get; set; }
        public string CounterName { get; set; } = null!;
    }
}
