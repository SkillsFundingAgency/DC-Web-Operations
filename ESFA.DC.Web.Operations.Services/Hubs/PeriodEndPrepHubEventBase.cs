using System;
using ESFA.DC.Web.Operations.Interfaces.PeriodEnd;
using ESFA.DC.Web.Operations.Services.Event;

namespace ESFA.DC.Web.Operations.Services.Hubs
{
    public sealed class PeriodEndPrepHubEventBase : IPeriodEndPrepHubEventBase
    {
        public event EventHandler PeriodEndHubPrepCallback;

        public void TriggerPeriodEndPrep(string contextConnectionId)
        {
            var handler = PeriodEndHubPrepCallback;
            handler?.Invoke(this, new SignalrEventArgs(contextConnectionId));
        }
    }
}
