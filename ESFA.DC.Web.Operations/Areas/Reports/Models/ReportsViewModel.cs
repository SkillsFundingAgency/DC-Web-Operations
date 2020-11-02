using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ESFA.DC.CollectionsManagement.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ESFA.DC.Web.Operations.Areas.Reports.Models
{
    public class ReportsViewModel
    {
        public string Collection { get; set; }

        public int CollectionYear { get; set; }

        public int CollectionPeriod { get; set; }

        public long? JobId { get; set; }

        public string ReportName { get; set; }

        public IEnumerable<SelectListItem> Reports { get; set; }

        public IEnumerable<ReturnPeriod> ReturnPeriods { get; set; }

        public Dictionary<int, List<SelectListItem>> Periods
        {
            get
            {
                if (ReturnPeriods == null || !ReturnPeriods.Any())
                {
                    return new Dictionary<int, List<SelectListItem>>();
                }

                var grouping = ReturnPeriods.GroupBy(k => k.CollectionYear).ToDictionary(g => g.Key, g => g.ToList());
                return grouping.ToDictionary(a => a.Key, a => a.Value.Select(s => new SelectListItem($"R{s.PeriodNumber:00}", s.PeriodNumber.ToString(CultureInfo.CurrentCulture))).ToList());
            }
        }

        public IEnumerable<int> CollectionYears { get; set; }

        public List<SelectListItem> Years => CollectionYears?
            .Select(n => new SelectListItem { Text = $"20{n.ToString(CultureInfo.CurrentCulture).Substring(0, 2)} to 20{n.ToString(CultureInfo.CurrentCulture).Substring(2, 2)}", Value = n.ToString(CultureInfo.CurrentCulture) })
            .OrderByDescending(o => o.Text)
            .ToList();

        public string ValidationReportGenerationUrl { get; set; }

        public string ReportsUrl { get; set; }

        public string ReportGenerationUrl { get; set; }

        public string ReportsDownloadUrl { get; set; }
    }
}
