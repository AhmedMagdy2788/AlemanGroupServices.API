using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlemanGroupServices.Core.Models
{
    public class TblCustomerTurck
    {
        [Key]
        [Column(Order = 1)]
        [ForeignKey("Tblcustomer")]
        public  int Customer_Id { get; set; }
        public string Name { get; set; } = null!;
        //[Key]
        //[Column(Order = 2)]
        public string Truck_No { get; set; } = null!;
    }
}
