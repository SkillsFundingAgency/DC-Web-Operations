using System.Collections.Generic;
using ESFA.DC.Web.Operations.Models.PeriodEnd;

namespace ESFA.DC.Web.Operations.Areas.PeriodEnd.Models
{
    public class PeriodEndReportViewModel
    {
        public PeriodEndReportViewModel()
        {
            ReportDetails = new List<ReportDetails>();
        }

        public int Period { get; set; }

        public int Year { get; set; }

        public IEnumerable<ReportDetails> ReportDetails { get; set; }

        public IEnumerable<ReportDetails> SampleReports { get; set; }
    }
}