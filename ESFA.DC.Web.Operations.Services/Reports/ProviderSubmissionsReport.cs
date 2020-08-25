using ESFA.DC.Web.Operations.Models.Enums;
using ESFA.DC.Web.Operations.Models.Reports;
using ESFA.DC.Web.Operations.Security.Policies;
using ESFA.DC.Web.Operations.Utils;

namespace ESFA.DC.Web.Operations.Services.Reports
{
    public class ProviderSubmissionsReport : IReport
    {
        public string ReportName => "ProviderSubmissionsReport";

        public string DisplayName => "Provider Submissions Report";

        public string CollectionName => Constants.ProviderSubmissionsReportCollectionName;

        public string ContainerName => Constants.ReportsBlobContainerName;

        public string Policy => AuthorisationPolicy.AdvancedSupportOrDevOpsPolicy;

        public ReportType ReportType => ReportType.PeriodEnd;
    }
}
