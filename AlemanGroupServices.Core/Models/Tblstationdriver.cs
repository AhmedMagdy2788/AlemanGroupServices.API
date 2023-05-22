using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AlemanGroupServices.Core.Models
{
    public partial class Tblstationdriver
    {
        [Key]
        public string CompanyDriversName { get; set; } = null!;
        public int LicenseNo { get; set; }
    }
}
