using System;
using ESFA.DC.Web.Operations.Interfaces.PeriodEnd;

namespace ESFA.DC.Web.Operations.Services.Hubs
{
    public class HubEventBase : IHubEventBase
    {
        public event EventHandler PeriodEndHubPrepCallback;

        public event EventHandler PeriodEndHubCallback;

        public void TriggerPeriodEnd()
        {
            var handler = PeriodEndHubCallback;
            handler?.Invoke(this, EventArgs.Empty);
        }

        public void TriggerPeriodEndPrep()
        {
            var handler = PeriodEndHubPrepCallback;
            handler?.Invoke(this, EventArgs.Empty);
        }
    }
}
