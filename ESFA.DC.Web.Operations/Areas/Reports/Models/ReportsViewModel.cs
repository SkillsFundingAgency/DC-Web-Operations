using System.Collections.Generic;
using ESFA.DC.Jobs.Model.Enums;
using ESFA.DC.PeriodEnd.Models;

namespace ESFA.DC.Web.Operations.Areas.Reports.Models
{
    public class ReportsViewModel
    {
        public string Collection { get; set; }

        public int CurrentCollectionYear { get; set; }

        public int CurrentCollectionPeriod { get; set; }

        public int CollectionYear { get; set; }

        public int CollectionPeriod { get; set; }

        public long? JobId { get; set; }

        public string ReportType { get; set; }

        public string ReportAction { get; set; }

        public JobStatusType ReportStatus { get; set; }

        public ReportDetails ReportDetails { get; set; }

        public IEnumerable<ReportDetails> Reports { get; set; }
    }
}
