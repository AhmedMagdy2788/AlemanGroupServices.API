using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AlemanGroupServices.Core.Models
{
    public partial class Tblproductscommission
    {
        [Key]
        [Column(Order = 1)]
        public DateTime Date { get; set; }
        [Key]
        [Column(Order = 2)]
        [ForeignKey("Tblproduct")]
        public int Product_Id { get; set; }
        public decimal Produc_Commission { get; set; }

        //public virtual Tblproduct ProductNameNavigation { get; set; } = null!;
    }

    public partial class ProductsCommissionDto
    {
        public DateTime Date { get; set; }
        public string Product_Name { get; set; } = null!;
        public decimal Product_Commission { get; set; }
    }
}
