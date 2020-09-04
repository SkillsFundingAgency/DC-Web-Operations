using ESFA.DC.Web.Operations.Models.Enums;
using ESFA.DC.Web.Operations.Models.Reports;
using ESFA.DC.Web.Operations.Security.Policies;
using ESFA.DC.Web.Operations.Utils;

namespace ESFA.DC.Web.Operations.Services.Reports
{
    public class PeriodEndDataQualityReport : IReport
    {
        public string ReportName => "PeriodEndDataQualityReport";

        public string DisplayName => "Period End Data Quality Report";

        public string CollectionName => CollectionNames.PeriodEndDataQualityReportCollectionName;

        public string ContainerName => Constants.ReportsBlobContainerName;

        public string Policy => AuthorisationPolicy.AdvancedSupportOrDevOpsPolicy;

        public ReportType ReportType => ReportType.PeriodEnd;
    }
}
