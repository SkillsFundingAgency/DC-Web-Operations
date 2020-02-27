using System;

namespace ESFA.DC.Web.Operations.Interfaces.Processing
{
    public interface IHubEventBase
    {
        event EventHandler ClientHeartbeatCallback;

        void ClientHeartbeat(string contextConnectionId);
    }
}
