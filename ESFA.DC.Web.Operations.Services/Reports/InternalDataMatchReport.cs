using ESFA.DC.Web.Operations.Models.Reports;
using ESFA.DC.Web.Operations.Security.Policies;

namespace ESFA.DC.Web.Operations.Services.Reports
{
    public class InternalDataMatchReport : IReport
    {
        public string ReportName => "InternalDataMatchReport";

        public string DisplayName => "Internal Data Match Report";

        public string CollectionName => "PE-DAS-AppsInternalDataMatchMonthEndReport{0}";

        public string Policy => AuthorisationPolicy.AdvancedSupportOrDevOpsPolicy;
    }
}
