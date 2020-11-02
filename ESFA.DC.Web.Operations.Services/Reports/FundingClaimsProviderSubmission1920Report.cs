using ESFA.DC.Web.Operations.Models.Enums;
using ESFA.DC.Web.Operations.Models.Reports;
using ESFA.DC.Web.Operations.Security.Policies;
using ESFA.DC.Web.Operations.Utils;

namespace ESFA.DC.Web.Operations.Services.Reports
{
    public class FundingClaimsProviderSubmission1920Report : IReport
    {
        public string ReportName => "FundingClaimsProviderSubmission1920Report";

        public string DisplayName => "1920 Funding Claims Provider Submissions Report";

        public string CollectionName => CollectionNames.FundingClaimsProviderSubmission1920Report;

        public string ContainerName => Constants.OpsReportsBlobContainerName;

        public string Policy => AuthorisationPolicy.AdvancedSupportOrDevOpsOrReportsPolicy;

        public ReportType ReportType => ReportType.FundingClaims;

        public bool IsApplicableForClosedPeriodOnly => false;
    }
}
