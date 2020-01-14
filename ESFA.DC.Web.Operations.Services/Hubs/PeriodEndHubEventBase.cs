using System;
using ESFA.DC.Web.Operations.Interfaces.PeriodEnd;
using ESFA.DC.Web.Operations.Services.Event;

namespace ESFA.DC.Web.Operations.Services.Hubs
{
    public sealed class PeriodEndHubEventBase : IPeriodEndHubEventBase
    {
        public event EventHandler PeriodEndHubPrepCallback;

        public event EventHandler PeriodEndHubCallback;

        public void TriggerPeriodEnd(string contextConnectionId)
        {
            var handler = PeriodEndHubCallback;
            handler?.Invoke(this, new SignalrEventArgs(contextConnectionId));
        }

        public void TriggerPeriodEndPrep(string contextConnectionId)
        {
            var handler = PeriodEndHubPrepCallback;
            handler?.Invoke(this, new SignalrEventArgs(contextConnectionId));
        }
    }
}
