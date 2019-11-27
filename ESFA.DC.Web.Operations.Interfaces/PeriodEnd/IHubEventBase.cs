using System;

namespace ESFA.DC.Web.Operations.Interfaces.PeriodEnd
{
    public interface IHubEventBase
    {
        event EventHandler PeriodEndHubPrepCallback;

        event EventHandler PeriodEndHubCallback;

        void TriggerPeriodEnd(string contextConnectionId);

        void TriggerPeriodEndPrep(string contextConnectionId);
    }
}