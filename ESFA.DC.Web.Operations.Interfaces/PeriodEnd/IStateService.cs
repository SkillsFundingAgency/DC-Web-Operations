using System.Collections.Generic;
using ESFA.DC.Jobs.Model;
using ESFA.DC.PeriodEnd.Models.Dtos;

namespace ESFA.DC.Web.Operations.Interfaces.PeriodEnd
{
    public interface IStateService
    {
        bool PauseReferenceDataIsEnabled(IEnumerable<JobSchedule> referenceDataJson);

        PeriodEndStateModel GetMainState(string pathItemStates);

        bool CollectionClosedEmailSent(PeriodEndStateModel periodEndStateModel);

        PeriodEndPrepModel GetPrepState(string state);

        bool AllJobsHaveCompleted(PeriodEndStateModel state);
    }
}