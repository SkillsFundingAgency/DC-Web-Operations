using System;

namespace ESFA.DC.Web.Operations.Interfaces.Processing
{
    public interface IJobSlowFileHubEventBase
    {
        event EventHandler ClientHeartbeatCallback;

        void ClientHeartbeat(string contextConnectionId);
    }
}
