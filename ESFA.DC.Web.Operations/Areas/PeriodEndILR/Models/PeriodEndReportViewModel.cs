using System.Collections.Generic;
using ESFA.DC.PeriodEnd.Models;
using ESFA.DC.Web.Operations.Models.Summarisation;

namespace ESFA.DC.Web.Operations.Areas.PeriodEndILR.Models
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

        public IEnumerable<ReportDetails> McaReports { get; set; }

        public IEnumerable<CollectionStats> CollectionStats { get; set; }

        public IEnumerable<SummarisationTotal> SummarisationTotals { get; set; }
    }
}