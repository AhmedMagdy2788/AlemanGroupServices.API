using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlemanGroupServices.Core.Models
{
    public class OilsSalesView
    {
        public DateTime Date { get; set; }
        public string Station_Name { get; set; } = null!;
        public string Product_Name { get; set; } = null!;
        public uint Quantity { get; set; }
        public double product_selling_price { get; set; }
        public double Total_Price { get; set; }
    }
}
