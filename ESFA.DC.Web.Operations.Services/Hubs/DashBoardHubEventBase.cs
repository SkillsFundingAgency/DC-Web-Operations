using System;
using ESFA.DC.Web.Operations.Interfaces.Dashboard;
using ESFA.DC.Web.Operations.Services.Event;

namespace ESFA.DC.Web.Operations.Services.Hubs
{
    public sealed class DashBoardHubEventBase : IDashBoardHubEventBase
    {
        public event EventHandler ClientHeartbeatCallback;

        public void ClientHeartbeat(string contextConnectionId)
        {
            var handler = ClientHeartbeatCallback;
            handler?.Invoke(this, new SignalrEventArgs(contextConnectionId));
        }
    }
}
