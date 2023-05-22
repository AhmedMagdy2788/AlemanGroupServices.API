using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AlemanGroupServices.Core.Models
{
    public partial class Tblwarehouse
    {
        [Key]
        public string Warehouse { get; set; } = null!;
        public int Marketing_Company_Id { get; set; } 
        public int Warehouse_Region_Id { get; set; }

        //public virtual MarketingCompany MarketingCompanyNavigation { get; set; } = null!;
        //public virtual Tblsourceregion? WarehouseRegionNavigation { get; set; }
    }

    public partial class WarehouseDto
    {
        public string Warehouse { get; set; } = null!;
        public string Marketing_Company_Name { get; set; } = null!;
        public string Warehouse_Region_Name { get; set; } = null!;

    }
}
