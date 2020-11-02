using ESFA.DC.Web.Operations.Models.Enums;
using ESFA.DC.Web.Operations.Models.Reports;
using ESFA.DC.Web.Operations.Security.Policies;
using ESFA.DC.Web.Operations.Utils;

namespace ESFA.DC.Web.Operations.Services.Reports
{
    public class RuleValidationDetailReport : IReport
    {
        public string ReportName => "RuleValidationDetailReport";

        public string DisplayName => "Rule Validation Detail Report";

        public string CollectionName => CollectionNames.ValidationRuleDetailsReportCollectionName;

        public string ContainerName => Constants.OpsReportsBlobContainerName;

        public string Policy => AuthorisationPolicy.ReportsPolicy;

        public ReportType ReportType => ReportType.Validation;

        public bool IsApplicableForClosedPeriodOnly => false;
    }
}
