using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
