using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AlemanGroupServices.Core.Models
{
    public partial class Tblproduct
    {
        [Key]
        public int Id { get; set; }
        public string Product_Name { get; set; } = null!;
        public int Category_Id { get; set; }
        //public virtual ICollection<Tblproductsbuyprices> Tblproductsbuyprices { get; set; } = null!;
    }

    public partial class ProductWithCategoryNameDTO
    {
        [Key]
        public int Id { get; set; }
        public string Product_Name { get; set; } = null!;
        public string Products_Category { get; set; } = null!;
    }

    public partial class PureProductDto
    {
        [Key]
        public int Id { get; set; }
        public string Product_Name { get; set; } = null!;
        public int Category_Id { get; set; }
    }
}
