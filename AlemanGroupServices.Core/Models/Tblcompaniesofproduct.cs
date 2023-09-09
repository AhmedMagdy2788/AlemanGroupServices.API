using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlemanGroupServices.Core.Models
{
    public partial class Tblcompaniesofproduct
    {
        public DateTime Date { get; set; }
        [Key]
        [Column(Order = 1)]
        [ForeignKey("Tblproduct")]
        public int Product_Id { get; set; }
        [Key]
        [Column(Order = 2)]
        [ForeignKey("MarketingCompany")]
        public Guid Source_Company_Id { get; set; }

        //public virtual Tblproduct ProductNameNavigation { get; set; } = null!;
        //public virtual MarketingCompany SourceCompanyNavigation { get; set; } = null!;
    }

    public partial class CompanyOfProductDto
    {
        public DateTime Date { get; set; }
        public string Product_Name { get; set; } = null!;
        public string Source_Company_Name { get; set; } = null!;
    }
}
