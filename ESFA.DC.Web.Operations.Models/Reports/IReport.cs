using ESFA.DC.Web.Operations.Security.Policies;

namespace ESFA.DC.Web.Operations.Models.Reports
{
    public interface IReport
    {
        string ReportName { get; }

        string DisplayName { get; }

        string CollectionName { get; }

        string Policy { get; }
    }
}
