using System;
using System.ComponentModel.DataAnnotations;

namespace ESFA.DC.Web.Operations.Areas.Collections.Models
{
    public class ManageReturnPeriodViewModel
    {
        public int ReturnPeriodId { get; set; }

        public int CollectionId { get; set; }

        public string CollectionName { get; set; }

        public string PeriodName { get; set; }

        [Required]
        public DateTime OpeningDate { get; set; }

        [Required]
        [RegularExpression("^([0-9]|0[0-9]|1[0-9]|2[0-3]):[0-5][0-9]$", ErrorMessage = "Invalid time format. hh:mm (24hr)")]
        public string OpeningTime { get; set; }

        [Required]
        public DateTime ClosingDate { get; set; }

        [Required]
        [RegularExpression("^([0-9]|0[0-9]|1[0-9]|2[0-3]):[0-5][0-9]$", ErrorMessage = "Invalid time format. hh:mm (24hr)")]
        public string ClosingTime { get; set; }
    }
}
