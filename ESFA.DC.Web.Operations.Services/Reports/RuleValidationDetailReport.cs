using ESFA.DC.Web.Operations.Models.Reports;
using ESFA.DC.Web.Operations.Security.Policies;

namespace ESFA.DC.Web.Operations.Services.Reports
{
    public class RuleValidationDetailReport : IReport
    {
        public string ReportName => "RuleValidationDetailReport";

        public string DisplayName => "Rule Validation Detail Report";

        public string CollectionName => "ILR{0}";

        public string Policy => AuthorisationPolicy.ReportsPolicy;
    }
}
