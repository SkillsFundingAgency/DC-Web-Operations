using System;

namespace ESFA.DC.Web.Operations.Interfaces.PeriodEnd
{
    public interface IPeriodEndPrepHubEventBase
    {
        event EventHandler PeriodEndHubPrepCallback;

        void TriggerPeriodEndPrep(string contextConnectionId);
    }
}