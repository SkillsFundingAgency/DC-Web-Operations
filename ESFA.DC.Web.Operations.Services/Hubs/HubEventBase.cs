using System;
using ESFA.DC.Web.Operations.Interfaces;
using ESFA.DC.Web.Operations.Services.Event;

namespace ESFA.DC.Web.Operations.Services.Hubs
{
    public sealed class HubEventBase : IHubEventBase
    {
        public event EventHandler HubCallback;

        public void TriggerHub(string contextConnectionId)
        {
            var handler = HubCallback;
            handler?.Invoke(this, new SignalrEventArgs(contextConnectionId));
        }
    }
}
