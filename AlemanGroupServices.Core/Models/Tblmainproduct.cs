using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AlemanGroupServices.Core.Models
{
    public partial class Tblmainproduct
    {
        [Key]
        public int CategoryId { get; set; }
        public string Products_Category { get; set; } = null!;
    }
}
