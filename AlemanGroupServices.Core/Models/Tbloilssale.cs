using AlemanGroupServices.Core.Const;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AlemanGroupServices.Core.Models
{
    public partial class Tbloilssale
    {
        [Key]
        [Column(Order = 3)]
        public DateTime Date { get; set; }
        [Key]
        [Column(Order = 1)]
        [ForeignKey("Tblstation")]
        public string StationName { get; set; } = null!;
        [Key]
        [Column(Order = 2)]
        [ForeignKey("Tblproduct")]
        public string OilName { get; set; } = null!;
        public int Quantity { get; set; }

        public virtual Tblproduct OilNameNavigation { get; set; } = null!;
        public virtual Station StationNameNavigation { get; set; } = null!;
    }
}
