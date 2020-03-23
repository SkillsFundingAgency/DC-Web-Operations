namespace ESFA.DC.Web.Operations.Areas.Frm.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using ESFA.DC.PeriodEnd.Models.Dtos;

    public class FrmReportModel
    {
        public bool IsFrmReportChoice { get; set; }

        [RegularExpression("[0-9]{4}", ErrorMessage = "The year code must be 4 numeric digits")]
        [Required(ErrorMessage = "A year code is required")]
        public int FrmYearPeriod { get; set; }

        public DateTime FrmDate { get; set; }

        public string FrmPeriod { get; set; }

        public DateTime? FrmCSVValidDate { get; set; }

        public long FrmJobId { get; set; }

        public string FrmJobType { get; set; }

        [Range(1, 14, ErrorMessage = "The period must be 2 numeric digits and be equal to a valid period")]
        [Required(ErrorMessage = "A period number is required")]
        public int FrmPeriodNumber { get; set; }

        public IEnumerable<PeriodEndCalendarYearAndPeriodModel> PublishedFrm { get; set; }
    }
}
