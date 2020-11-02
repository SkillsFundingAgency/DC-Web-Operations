using ESFA.DC.Web.Operations.Models.Enums;
using ESFA.DC.Web.Operations.Models.Reports;
using ESFA.DC.Web.Operations.Security.Policies;
using ESFA.DC.Web.Operations.Utils;

namespace ESFA.DC.Web.Operations.Services.Reports
{
    public class ACTCountReport : IReport
    {
        public string ReportName => "ACTCountReport";

        public string DisplayName => "ACT Count Report";

        public string CollectionName => CollectionNames.ActCountReportCollectionName;

        public string ContainerName => Constants.ReportsBlobContainerName;

        public string Policy => AuthorisationPolicy.AdvancedSupportOrDevOpsPolicy;

        public ReportType ReportType => ReportType.PeriodEnd;

        public bool IsApplicableForClosedPeriodOnly => false;
    }
}
