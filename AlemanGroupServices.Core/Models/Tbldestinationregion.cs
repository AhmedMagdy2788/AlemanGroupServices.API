using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AlemanGroupServices.Core.Models
{
    public partial class Tbldestinationregion
    {
        [Key]
        public string UnloadingRegions { get; set; } = null!;
    }
}
