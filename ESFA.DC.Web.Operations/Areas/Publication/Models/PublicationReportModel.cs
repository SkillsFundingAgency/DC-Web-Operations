using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ESFA.DC.PeriodEnd.Models.Dtos;

namespace ESFA.DC.Web.Operations.Areas.Publication.Models
{
    public class PublicationReportModel
    {
        public bool IsFrmReportChoice { get; set; }

        //[RegularExpression("[0-9]{4}", ErrorMessage = "The year code must be 4 numeric digits")]
        //[Required(ErrorMessage = "A year code is required")]
        public int PublicationYearPeriod { get; set; }

        public DateTime PublicationDate { get; set; }

        public string FrmPeriod { get; set; }

        public DateTime? FrmCSVValidDate { get; set; }

        public long FrmJobId { get; set; }

        public string FrmJobType { get; set; }

        [Range(1, 14, ErrorMessage = "The period must be 2 numeric digits and be equal to a valid period")]
        [Required(ErrorMessage = "A period number is required")]
        public int PeriodNumber { get; set; }

        [Required(ErrorMessage = "Collection name is required")]
        public string CollectionName { get; set; }

        public IEnumerable<PeriodEndCalendarYearAndPeriodModel> PublishedFrm { get; set; }
    }
}
