using AlemanGroupServices.Core.Const;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AlemanGroupServices.Core.Models
{
    
    public partial class Tblproductsbuyprices
    {
        [Key]
        [Column(Order = 1)]
        public DateTime Date { get; set; }
        [Key]
        [Column(Order = 2)]
        [ForeignKey("Tblproduct")]
        public int Product_Id { get; set; }
        public decimal Product_Purchase_Price { get; set; }

        //public virtual Tblproduct ProductNavigation { get; set; } = null!;
    }

    public partial class ProductsPurchasePriceDto
    {
        public DateTime Date { get; set; }
        public int Product_Id { get; set; }
        public decimal Product_Purchase_Price { get; set; }
    }


    public partial class ProductsPurchasePriceWithProductNameDto
    {
        public DateTime Date { get; set; }
        public string Product_Name { get; set; } = null!;
        public decimal Product_Purchase_Price { get; set; }
    }
}
