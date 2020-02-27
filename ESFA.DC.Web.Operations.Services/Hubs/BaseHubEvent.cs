using System;
using ESFA.DC.Web.Operations.Services.Event;

namespace ESFA.DC.Web.Operations.Services.Hubs
{
    public abstract class BaseHubEvent
    {
        public event EventHandler ClientHeartbeatCallback;

        public virtual void ClientHeartbeat(string contextConnectionId)
        {
            var handler = ClientHeartbeatCallback;
            handler?.Invoke(this, new SignalrEventArgs(contextConnectionId));
        }
    }
}
