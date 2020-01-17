using ESFA.DC.PeriodEnd.Models.Dtos;
using ESFA.DC.Web.Operations.Interfaces.PeriodEnd;
using ESFA.DC.Web.Operations.Models;

namespace ESFA.DC.Web.Operations.Services
{
    public class PeriodEndStateFactory : IPeriodEndStateFactory
    {
        public PeriodEndState GetPeriodEndState(PeriodEndStateModel stateModel)
        {
            var state = new PeriodEndState
            {
                McaReportsPublished = stateModel.McaReportsPublished,
                ProviderReportsPublished = stateModel.ProviderReportsPublished,
                PeriodEndClosed = stateModel.PeriodEndFinished,
                PeriodEndStarted = stateModel.PeriodEndStarted,
                ReferenceDataJobsPaused = stateModel.ReferenceDataJobsPaused,
                McaReportsReady = stateModel.McaReportsReady,
                ProviderReportsReady = stateModel.ProviderReportsReady,
                CollectionClosedEmailSent = stateModel.CollectionClosedEmailSent
            };

            return state;
        }
    }
}