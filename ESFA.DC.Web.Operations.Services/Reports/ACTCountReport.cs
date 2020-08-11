using ESFA.DC.Web.Operations.Models.Reports;
using ESFA.DC.Web.Operations.Security.Policies;

namespace ESFA.DC.Web.Operations.Services.Reports
{
    public class ACTCountReport : IReport
    {
        public string ReportName => "ACTCountReport";

        public string DisplayName => "ACT Count Report";

        public string CollectionName => "PE-ACT-Count-Report1920";

        public string Policy => AuthorisationPolicy.AdvancedSupportOrDevOpsPolicy;
    }
}
