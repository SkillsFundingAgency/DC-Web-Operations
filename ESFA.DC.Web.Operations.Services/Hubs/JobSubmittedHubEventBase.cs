using System;
using ESFA.DC.Web.Operations.Interfaces.Processing;
using ESFA.DC.Web.Operations.Services.Event;

namespace ESFA.DC.Web.Operations.Services.Hubs
{
    public sealed class JobSubmittedHubEventBase : IJobSubmittedHubEventBase
    {
        public event EventHandler ClientHeartbeatCallback;

        public void ClientHeartbeat(string contextConnectionId)
        {
            var handler = ClientHeartbeatCallback;
            handler?.Invoke(this, new SignalrEventArgs(contextConnectionId));
        }
    }
}
