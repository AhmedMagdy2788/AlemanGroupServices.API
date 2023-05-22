using System.ComponentModel.DataAnnotations;

namespace AlemanGroupServices.Core.Models
{
    public partial class MarketingCompny
    {
        //public MarketingCompany()
        //{
        //    Tblstations = new HashSet<Tblstation>();
        //    Tblwarehouses = new HashSet<Tblwarehouse>();
        //}
        [Key]
        public int Id { get; set; }
        public string Marketing_comany { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string Fax { get; set; } = null!;
        public string Email { get; set; } = null!;

        //public virtual ICollection<Tblstation> Tblstations { get; set; }
        //public virtual ICollection<Tblwarehouse> Tblwarehouses { get; set; }
    }
}
