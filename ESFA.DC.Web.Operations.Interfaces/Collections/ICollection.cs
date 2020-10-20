namespace ESFA.DC.Web.Operations.Interfaces.Collections
{
    public interface ICollection
    {
        string CollectionName { get; }

        string ReportName { get; }

        string DisplayName { get; }

        string HubName { get; }

        string FileFormat { get; }

        string FileNameFormat { get; }

        bool IsDisplayedOnReferenceDataSummary { get; }
    }
}
