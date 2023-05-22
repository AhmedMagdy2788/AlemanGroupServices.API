using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AlemanGroupServices.Core.Models
{
    public partial class TransportationCompany
    {
        [Key]
        public uint Id { get; set; }
        public string TransportContractors { get; set; } = null!;
    }
}
