using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ESFA.DC.Jobs.Model.Enums;
using ESFA.DC.PeriodEnd.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

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

        public IDictionary<string, int> ReportPeriods { get; set; }

        public List<SelectListItem> Periods => ReportPeriods
            .Select(n => new SelectListItem { Text = n.Key, Value = n.Value.ToString(CultureInfo.CurrentCulture) })
            .OrderBy(o => o.Text)
            .ToList();

        public IEnumerable<int> CollectionYears { get; set; }

        public List<SelectListItem> Years => CollectionYears
            .Select(n => new SelectListItem { Text = n.ToString(CultureInfo.CurrentCulture), Value = n.ToString(CultureInfo.CurrentCulture) })
            .OrderByDescending(o => o.Text)
            .ToList();
    }
}
