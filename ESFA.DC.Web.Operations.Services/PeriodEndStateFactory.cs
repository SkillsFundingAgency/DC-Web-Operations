using ESFA.DC.PeriodEnd.Models.Dtos;
using ESFA.DC.Web.Operations.Interfaces.PeriodEnd;
using ESFA.DC.Web.Operations.Models;

namespace ESFA.DC.Web.Operations.Services
{
    public class PeriodEndStateFactory : IPeriodEndStateFactory
    {
        public PeriodEndState GetPeriodEndState(PathPathItemsModel pathModel)
        {
            var state = new PeriodEndState
            {
                McaReportsPublished = pathModel.McaReportsPublished,
                ProviderReportsPublished = pathModel.ProviderReportsPublished,
                PeriodEndClosed = pathModel.PeriodEndFinished,
                PeriodEndStarted = pathModel.PeriodEndStarted,
                ReferenceDataJobsPaused = pathModel.ReferenceDataJobsPaused,
                McaReportsReady = pathModel.McaReportsReady,
                ProviderReportsReady = pathModel.ProviderReportsReady,
                CollectionClosedEmailSent = pathModel.CollectionClosedEmailSent
            };

            return state;
        }
    }
}