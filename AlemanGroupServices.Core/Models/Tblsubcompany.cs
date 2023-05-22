using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AlemanGroupServices.Core.Models
{
    public partial class Tblsubcompany
    {
        public Tblsubcompany()
        {
            Tblstations = new HashSet<Station>();
        }

        [Key]
        public int Id { get; set; }
        public string Subcompany_name { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string Tax_card { get; set; } = null!;
        public string Commercial_registration { get; set; } = null!;
        public string Fax { get; set; } = null!;
        public string Email { get; set; } = null!;

        public virtual ICollection<Station> Tblstations { get; set; }
    }
}
