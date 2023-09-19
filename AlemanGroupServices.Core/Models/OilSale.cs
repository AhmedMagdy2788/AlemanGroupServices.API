using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlemanGroupServices.Core.Models
{
    public partial class OilSale
    {
        [Key] public int Id { get; set; }
        public DateTime Date { get; set; }
        [ForeignKey("Tblstation")]
        public Guid StationId { get; set; }
        [ForeignKey("Tblproduct")]
        public int OilId { get; set; }
        public int Quantity { get; set; }

        //public virtual Tblproduct OilNameNavigation { get; set; } = null!;
        //public virtual Station StationNameNavigation { get; set; } = null!;
    }
    public partial class OilSaleDto
    {
        public int? Id { get; set; }
        public DateTime Date { get; set; }
        public string StationName { get; set; } = null!;
        public string OilName { get; set; } = null!;
        public int Quantity { get; set; }
    }
}
