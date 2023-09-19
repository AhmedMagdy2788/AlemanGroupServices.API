using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlemanGroupServices.Core.Models
{
    public class AccountsInterfaces
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("Subcompany")]
        public Guid subcompany_id { get; set; }
        public string accounts_interfaces { get; set; } = null!;
    }


    public class AccountsInterfacesDto
    {
        public int Id { get; set; }
        public string subcompany_name { get; set; } = null!;
        public string accounts_interfaces { get; set; } = null!;
    }
}
