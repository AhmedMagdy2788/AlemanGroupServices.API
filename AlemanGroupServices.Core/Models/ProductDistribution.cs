using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlemanGroupServices.Core.Models
{
    public class ProductDistribution
    {
        public DateTime Date { get; set; }
        [Key]
        [ForeignKey("WithdrawalFromMarketingCompany")]
        public uint OrderNo { get; set; }
        [Key]
        [ForeignKey("Tblproduct")]
        public int ProductId { get; set; }
        [Key]
        [ForeignKey("DestinationRegion")]
        public int DestinationId { get; set; }
        public double Quantity { get; set; }
    }

    public class ProductDistributionDto
    {
        public DateTime Date { get; set; }
        public uint OrderNo { get; set; }
        public string ProductName { get; set; } = null!;
        public string DestinationName { get; set; } = null!;
        public double Quantity { get; set; }
    }


    public class ProductDistributionGroupedByProductNameTillDateDto
    {
        public DateTime TillDate { get; set; }
        public string GroupProductName { get; set; } = null!;
        public string DestinationName { get; set; } = null!;
        public double QuantitiesSum { get; set; }
    }
}
