namespace ESFA.DC.Web.Operations.Services.Collections
{
    public abstract class BaseCollection
    {
        public abstract string CollectionName { get; }

        public virtual bool IsDisplayedOnReferenceDataSummary => true;
    }
}
