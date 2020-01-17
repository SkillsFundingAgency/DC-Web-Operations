using ESFA.DC.PeriodEnd.Models.Dtos;
using ESFA.DC.Web.Operations.Models;

namespace ESFA.DC.Web.Operations.Interfaces.PeriodEnd
{
    public interface IStateService
    {
        bool PauseReferenceDataIsEnabled(string referenceDataJson);

        PeriodEndStateModel GetState(string pathItemStates);

        bool CollectionClosedEmailSent(PeriodEndStateModel periodEndStateModel);

        PeriodEndState GetPeriodEndState(PeriodEndStateModel periodEndStateModel);
    }
}