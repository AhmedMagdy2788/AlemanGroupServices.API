using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AlemanGroupServices.Core.Models
{
    public partial class Tblpumptype
    {
        [Key]
        public string TanksProducts { get; set; } = null!;

        public virtual Tblproduct TanksProductsNavigation { get; set; } = null!;
    }
}
