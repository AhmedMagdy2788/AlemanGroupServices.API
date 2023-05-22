using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AlemanGroupServices.Core.Models
{
    public partial class Tblsourceregion
    {
        //public Tblsourceregion()
        //{
        //    Tblwarehouses = new HashSet<Tblwarehouse>();
        //}

        [Key]
        public int Id { get; set; }
        public string Warehouse_Region { get; set; } = null!;

        //public virtual ICollection<Tblwarehouse> Tblwarehouses { get; set; }
    }
}
