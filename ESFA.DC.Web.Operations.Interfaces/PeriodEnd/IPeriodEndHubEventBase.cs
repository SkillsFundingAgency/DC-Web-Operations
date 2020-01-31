using System;

namespace ESFA.DC.Web.Operations.Interfaces.PeriodEnd
{
    public interface IPeriodEndHubEventBase
    {
        event EventHandler PeriodEndHubCallback;

        void TriggerPeriodEnd(string contextConnectionId);
    }
}