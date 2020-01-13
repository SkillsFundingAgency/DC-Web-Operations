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

        [RegularExpression("[0-9]{4}", ErrorMessage = "The year code must be 4 numeric digits")]
        [Required(ErrorMessage = "A year code is required")]
        public string FrmYearPeriod { get; set; }

        [Required(ErrorMessage = "A start date is required")]
        public DateTime FrmDate { get; set; }

        public string FrmPeriod { get; set; }

        public DateTime? FrmCSVValidDate { get; set; }
    }
}
