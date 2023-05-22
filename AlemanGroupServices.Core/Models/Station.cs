using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlemanGroupServices.Core.Models
{

    public class Station
    {
        [Key]
        public int Station_Id { get; set; }
        public string Station_Name { get; set; } = null!;
        public string Location { get; set; } = null!;
        [ForeignKey("Tblsubcompany")]
        public int Owner_company_Id { get; set; }
        [ForeignKey("MarketingCompany")]
        public int? Partner_ship_Id { get; set; }
    }

    public class StationDto
    {
        [Key]
        public int Station_id { get; set; }
        public string Station_name { get; set; } = null!;
        public string Location { get; set; } = null!;
        public string Owner_company { get; set; } = null!;
        public string? Partner_ship { get; set; }
    }

    public class StationIdNamePairs
    {
        [Key]
        public int Station_id { get; set; }
        public string Station_name { get; set; } = null!;
    }
}
