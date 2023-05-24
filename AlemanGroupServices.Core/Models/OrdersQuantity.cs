using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AlemanGroupServices.Core.Models
{
    public class OrdersQuantity
    {
        [Key]
        [ForeignKey("WithdrawalFromMarketingCompany")]
        public uint OrderNo { get; set; }
        [Key]
        [ForeignKey("Tblproduct")]
        public int ProductId { get; set; }
        public double Quantity { get; set; }   
    }

    public class OrdersQuantityDto
    {
        public uint OrderNo { get; set; }
        public string ProductName { get; set; } = null!;
        public double Quantity { get; set; }
    }
}
