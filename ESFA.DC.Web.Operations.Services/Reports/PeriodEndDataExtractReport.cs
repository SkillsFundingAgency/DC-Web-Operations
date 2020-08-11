using ESFA.DC.Web.Operations.Models.Reports;
using ESFA.DC.Web.Operations.Security.Policies;

namespace ESFA.DC.Web.Operations.Services.Reports
{
    public class PeriodEndDataExtractReport : IReport
    {
        public string ReportName => "PeriodEndDataExtractReport";

        public string DisplayName => "Period End Data Extract Report";

        public string CollectionName => "PE-DataExtract-Report{0}";

        public string Policy => AuthorisationPolicy.AdvancedSupportOrDevOpsPolicy;
    }
}
