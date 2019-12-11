using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ESFA.DC.Web.Operations.Areas.Frm.Models
{
    public class FrmReportModel
    {
        public bool IsFrmReportChoice { get; set; }

        [StringLength(4, ErrorMessage = "The Year Code must be 4 digits long")]
        [RegularExpression("[0-9]{4}", ErrorMessage = "The year code must only contain numerics")]
        [Required(ErrorMessage = "A start date is required")]
        public string FrmYearPeriod { get; set; }

        [Required(ErrorMessage = "A start date is required")]
        public DateTime FrmDate { get; set; }
    }
}
