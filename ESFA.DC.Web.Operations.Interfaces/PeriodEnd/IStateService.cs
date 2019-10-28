namespace ESFA.DC.Web.Operations.Interfaces.PeriodEnd
{
    public interface IStateService
    {
        bool PauseReferenceDataIsEnabled(string referenceDataJson);

        bool CollectionClosedEmailSent(string pathItemStates);
    }
}