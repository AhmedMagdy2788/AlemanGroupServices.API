using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AlemanGroupServices.Core.Models
{
    public partial class Tblproductssalesprice
    {
        [Key]
        [Column(Order = 1)]
        public DateTime Date { get; set; }
        [Key]
        [Column(Order = 2)]
        [ForeignKey("Tblproduct")]
        public int Product_Id { get; set; } 
        public decimal Product_Selling_Price { get; set; }

        //public virtual Tblproduct ProductNameNavigation { get; set; } = null!;
    }

    public partial class ProductsSalePriceDto
    {
        public DateTime Date { get; set; }
        public int Product_Id { get; set; }
        public decimal Product_Selling_Price { get; set; }
    }


    public partial class ProductsSalePriceWithProductNameDto
    {
        public DateTime Date { get; set; }
        public string Product_Name { get; set; } = null!;
        public decimal Product_Selling_Price { get; set; }
    }
}
