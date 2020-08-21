using System;
using System.Collections.Generic;
using System.Text;
using ESFA.DC.PeriodEnd.Models;

namespace ESFA.DC.Web.Operations.Models.Reports
{
    public class ReportsModel
    {
        public IEnumerable<ReportDetails> ReportUrlDetails { get; set; }

        public IEnumerable<IReport> AvailableReportsList { get; set; }
    }
}
