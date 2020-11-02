using ESFA.DC.Web.Operations.Models.Enums;
using ESFA.DC.Web.Operations.Models.Reports;
using ESFA.DC.Web.Operations.Security.Policies;
using ESFA.DC.Web.Operations.Utils;

namespace ESFA.DC.Web.Operations.Services.Reports
{
    public class ILRProvidersReturningFirstTimePerDayReport : IReport
    {
        public string ReportName => "ILRProvidersReturningFirstTimePerDayReport";

        public string DisplayName => "Providers Returning First Time Per Day Report";

        public string CollectionName => CollectionNames.ILRProvidersReturningFirstTimePerDayReport;

        public string ContainerName => Constants.OpsReportsBlobContainerName;

        public string Policy => AuthorisationPolicy.ReportsPolicy;

        public ReportType ReportType => ReportType.Operations;

        public bool IsApplicableForClosedPeriodOnly => true;
    }
}
