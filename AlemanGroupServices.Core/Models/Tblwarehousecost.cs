using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AlemanGroupServices.Core.Models
{
    public partial class Tblwarehousecost
    {
        [Key]
        [Column(Order = 1)]
        public DateTime Date { get; set; }
        [Key]
        [Column(Order = 2)]
        [ForeignKey("Tblproduct")]
        public int Product_Id { get; set; }
        [Key]
        [Column(Order = 3)]
        [ForeignKey("Tblwarehouse")]
        public string Warehouse { get; set; } = null!;
        public double Warehouse_Expenses { get; set; }
        public double Expenses_On_Customer { get; set; }

        //public virtual Tblproduct ProductNameNavigation { get; set; } = null!;
        //public virtual Tblwarehouse WarehouseNavigation { get; set; } = null!;
    }

    public partial class WarehouseCostDto
    {
        public DateTime Date { get; set; }
        public string Product_Name { get; set; } = null!;
        public string Warehouse { get; set; } = null!;
        public double Warehouse_Expenses { get; set; }
        public double Expenses_On_Customer { get; set; }
    }
}
