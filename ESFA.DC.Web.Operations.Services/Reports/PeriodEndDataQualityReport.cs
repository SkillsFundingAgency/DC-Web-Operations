using ESFA.DC.Web.Operations.Models.Reports;
using ESFA.DC.Web.Operations.Security.Policies;

namespace ESFA.DC.Web.Operations.Services.Reports
{
    public class PeriodEndDataQualityReport : IReport
    {
        public string ReportName => "PeriodEndDataQualityReport";

        public string DisplayName => "Period End Data Quality Report";

        public string CollectionName => "PE-DataQuality-Report{0}";

        public string Policy => AuthorisationPolicy.AdvancedSupportOrDevOpsPolicy;
    }
}
