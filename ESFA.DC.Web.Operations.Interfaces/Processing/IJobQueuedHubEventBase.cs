using System;

namespace ESFA.DC.Web.Operations.Interfaces.Processing
{
    public interface IJobQueuedHubEventBase
    {
        event EventHandler ClientHeartbeatCallback;

        void ClientHeartbeat(string contextConnectionId);
    }
}
