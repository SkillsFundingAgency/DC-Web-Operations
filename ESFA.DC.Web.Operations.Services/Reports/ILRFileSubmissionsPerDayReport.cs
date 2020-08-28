using ESFA.DC.Web.Operations.Models.Enums;
using ESFA.DC.Web.Operations.Models.Reports;
using ESFA.DC.Web.Operations.Security.Policies;
using ESFA.DC.Web.Operations.Utils;

namespace ESFA.DC.Web.Operations.Services.Reports
{
    public class ILRFileSubmissionsPerDayReport : IReport
    {
        public string ReportName => "ILRFileSubmissionsPerDayReport";

        public string DisplayName => "ILR File Submissions Per Day Report";

        public string CollectionName => CollectionNames.ILRFileSubmissionsPerDayReport;

        public string ContainerName => Constants.OpsReportsBlobContainerName;

        public string Policy => AuthorisationPolicy.ReportsPolicy;

        public ReportType ReportType => ReportType.Operations;
    }
}
