using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlemanGroupServices.Core.Models
{
    public class MCAccountProduct
    {
        [Key]
        [ForeignKey("MarketingCompaniesAccounts")]
        public uint AccountNo { get; set; }
        [Key]
        [ForeignKey("Tblmainproduct")]
        public int MainProductId { get; set; }
        public override string  ToString()
        {
            return $"AccountNo: {AccountNo}, MainProductId: {MainProductId}";
        }
    }

    public class MCAccountProductDto
    {
        public uint AccountNo { get; set; }
        public string MainProductName { get; set; } = null!;
    }
}
