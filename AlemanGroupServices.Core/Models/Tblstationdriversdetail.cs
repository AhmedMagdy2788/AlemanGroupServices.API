using AlemanGroupServices.Core.Const;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AlemanGroupServices.Core.Models
{
    public partial class Tblstationdriversdetail
    {
        [Key]
        [Column(Order = 2)]
        public DateTime Date { get; set; }
        [Key]
        [Column(Order = 1)]
        [ForeignKey("Tblstationdriver")]
        public string DirverName { get; set; } = null!;

        public double DirverCommission { get; set; }

        public virtual Tblstationdriver DirverNameNavigation { get; set; } = null!;
    }
}
