using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESFA.DC.Web.Operations.Areas.Reports.Models
{
    public class ValidationRuleDetailReportViewModel
    {
        public long JobId { get; set; }

        public string ReportFileName { get; set; }

        public string ContainerName { get; set; }

        public string Rule { get; set; }

        public int Year { get; set; }

        public Dictionary<string, List<ValidationRuleDetail>> Validationruledetails { get; set; }
    }

    public class ValidationRuleDetail
    {
        public string ReturnPeriod { get; set; }

        public string ProviderName { get; set; }

        public int? UkPrn { get; set; }

        public int Errors { get; set; }

        public int Warnings { get; set; }

        public DateTime SubmissionDate { get; set; }
    }
}
