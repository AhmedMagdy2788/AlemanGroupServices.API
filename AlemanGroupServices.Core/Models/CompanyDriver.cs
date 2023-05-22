using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlemanGroupServices.Core.Models
{
    public class CompanyDriver
    {
        [Key]
        public uint Id { get; set; }
        public string DriverName { get; set; } = null!;
        public int? LicenseNo { get; set; } = null;
    }
}
