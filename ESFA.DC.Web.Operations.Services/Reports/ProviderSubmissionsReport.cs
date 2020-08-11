using ESFA.DC.Web.Operations.Models.Reports;
using ESFA.DC.Web.Operations.Security.Policies;

namespace ESFA.DC.Web.Operations.Services.Reports
{
    public class ProviderSubmissionsReport : IReport
    {
        public string ReportName => "ProviderSubmissionsReport";

        public string DisplayName => "Provider Submissions Report";

        public string CollectionName => "PE-ProviderSubmission-Report{0}";

        public string Policy => AuthorisationPolicy.AdvancedSupportOrDevOpsPolicy;
    }
}
