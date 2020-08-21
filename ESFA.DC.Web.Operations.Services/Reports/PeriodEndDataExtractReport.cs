using ESFA.DC.Web.Operations.Models.Enums;
using ESFA.DC.Web.Operations.Models.Reports;
using ESFA.DC.Web.Operations.Security.Policies;
using ESFA.DC.Web.Operations.Utils;

namespace ESFA.DC.Web.Operations.Services.Reports
{
    public class PeriodEndDataExtractReport : IReport
    {
        public string ReportName => "PeriodEndDataExtractReport";

        public string DisplayName => "Period End Data Extract Report";

        public string CollectionName => Constants.InternalDataMatchReportCollectionName;

        public string ContainerName => Constants.ReportsBlobContainerName;

        public string Policy => AuthorisationPolicy.AdvancedSupportOrDevOpsPolicy;

        public ReportType ReportType => ReportType.PeriodEnd;
    }
}
