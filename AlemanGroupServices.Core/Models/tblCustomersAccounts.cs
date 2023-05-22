using System.ComponentModel.DataAnnotations;

namespace AlemanGroupServices.Core.Models
{
    public class tblCustomersAccounts
    {
        [Key]
        public int Account_No { get; set; }
        public DateTime Date { get; set; }
        public int Customer_No { get; set; }
        public string Accounts_Interfaces { get; set; } = null!;
        public double Openning_Dept { get; set; } = 0;
    }
}
