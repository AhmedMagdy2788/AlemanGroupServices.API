using AlemanGroupServices.Core.Const;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AlemanGroupServices.Core.Models
{
    public partial class CountersFeedbackPercentage
    {
        [Key]
        [Column(Order = 1)]
        public DateTime Date { get; set; }
        [Key]
        [Column(Order = 2)]
        [ForeignKey("StationsCounter")]
        public uint CounterNo { get; set; }
        public double FeedbackPercentagePerGalon { get; set; }

        //public virtual StationsCounter CounterNoNavigation { get; set; } = null!;
    }
}
