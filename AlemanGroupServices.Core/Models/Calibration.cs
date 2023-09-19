using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlemanGroupServices.Core.Models
{
    public class Calibration
    {
        [Key]
        [Column(Order = 1)]
        public DateTime Date { get; set; }
        [Key]
        [Column(Order = 2)]
        [ForeignKey("Station")]
        public Guid Station_Id { get; set; }
        [Key]
        [Column(Order = 3)]
        [ForeignKey("tblpumptype")]
        public string Product_Name { get; set; } = null!;
        public double Quantity { get; set; }
    }

    public class CalibrationDto
    {
        public DateTime Date { get; set; }
        public string Station_Name { get; set; } = null!;
        public string Product_Name { get; set; } = null!;
        public double Quantity { get; set; }
    }
}
