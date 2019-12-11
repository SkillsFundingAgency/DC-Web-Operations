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

        [StringLength(4)]
        [Required(ErrorMessage = "A start date is required")]
        public string FrmYearPeriod { get; set; }

        [Required(ErrorMessage = "A start date is required")]
        public DateTime FrmDate { get; set; }
    }
}
