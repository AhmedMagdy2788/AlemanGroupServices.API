using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlemanGroupServices.Core.Models
{
    public class MarketingCompaniesAccounts
    {
        [Key]
        public uint AccountNo { get; set; }
        public DateTime Date { get; set; }
        [ForeignKey("AccountsInterfaces")]
        public int AccountInterfaceId { get; set; }
        [ForeignKey("MarketingCompany")]
        public Guid MarketingCompanyId { get; set; }
        public double InitialDept { get; set; }
    }

    public class MarketingCompaniesAccountsDto
    {
        public uint AccountNo { get; set; }
        public DateTime Date { get; set; }
        public string Subcompany_Name { get; set; } = null!;
        public string AccountsInterface { get; set; } = null!;
        public string MarketingCompany { get; set; } = null!;
        public double InitialDept { get; set; }
    }

    public class MarketingCompaniesAccountsPair
    {
        public uint AccountNo { get; set; }
        public string accountName { get; set; } = null!;
    }
}
