using System.Collections.Generic;
using ESFA.DC.Operations.Reports.Model;

namespace ESFA.DC.Web.Operations.Areas.Reports.Models
{
    public class ValidationRuleDetailReportViewModel
    {
        public long JobId { get; set; }

        public string ReportFileName { get; set; }

        public string ContainerName { get; set; }

        public string Rule { get; set; }

        public int Year { get; set; }

        public int Period { get; set; }

        public Dictionary<string, List<ValidationRuleDetail>> ValidationRuleDetailsByReturnPeriod { get; set; }

        public string ValidationReportGenerationUrl { get; set; }
    }
}
