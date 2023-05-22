using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AlemanGroupServices.Core.Models
{
	public class TanksEquilibrium
	{
        [Key]
        [Column(Order = 1)]
        public DateTime Date { get; set; }
        [Key]
        [Column(Order = 2)]
        [ForeignKey("Station")]
        public int Station_Id { get; set; }
        [Key]
        [Column(Order = 3)]
        [ForeignKey("tblpumptype")]
        public string  Product_Name { get; set; } = null!;
		public double Quantity { get; set; }
        public string? Notes { get; set; }
    }

    public class TanksEquilibriumDto
    {
        public DateTime Date { get; set; }
        public string Station_Name { get; set; } = null!;
        public string Product_Name { get; set; } = null!;
        public double Quantity { get; set; }
        public string? Notes { get; set; }
    }
}
