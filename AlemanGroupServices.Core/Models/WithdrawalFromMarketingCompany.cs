using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AlemanGroupServices.Core.Models
{
    public class WithdrawalFromMarketingCompany
    {
        [Key]
        public uint OrderNO { get; set; }
        public DateTime OrderDate { get; set; }
        [ForeignKey("MarketingCompaniesAccounts")]
        public uint AccountNo { get; set; }
        [ForeignKey("Warehouse")]
        public string Wareahouse { get; set; } = null!;
        public uint MCInvoiceNo { get; set; }
        public uint? WarehouseInvoiceNo { get; set; } = null;
        [ForeignKey("TransportationCompany")]
        public uint TransportationId { get; set; }
        [ForeignKey("CompanyTruck")]
        public uint TruckId { get; set; }
        [ForeignKey("CompanyDriver")]
        public uint DriverId { get; set; }
    }

    public class WithdrawalFromMarketingCompanyDto
    {
        public uint OrderNO { get; set; }
        public DateTime OrderDate { get; set; }
        public uint AccountNo { get; set; }
        public string? AccountName { get; set; }
        public string Wareahouse { get; set; } = null!;
        public uint MCInvoiceNo { get; set; }
        public uint? WarehouseInvoiceNo { get; set; } = null;
        public string TransportationName { get; set; } = null!;
        public string TruckNumber { get; set; } = null!;
        public string DriverName { get; set; } = null!;
    }
}
