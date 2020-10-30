using ESFA.DC.Web.Operations.Models.Enums;
using ESFA.DC.Web.Operations.Security.Policies;
using ESFA.DC.Web.Operations.Utils;

namespace ESFA.DC.Web.Operations.Models.Reports
{
    public interface IReport
    {
        string ReportName { get; }

        string DisplayName { get; }

        string CollectionName { get; }

        string ContainerName { get; }

        string Policy { get; }

        ReportType ReportType { get; }

        bool IsApplicableForClosedPeriodOnly { get; }
    }
}
