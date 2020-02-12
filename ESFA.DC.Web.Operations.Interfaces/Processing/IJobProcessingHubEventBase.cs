using System;

namespace ESFA.DC.Web.Operations.Interfaces.Processing
{
    public interface IJobProcessingHubEventBase
    {
        event EventHandler ClientHeartbeatCallback;

        void ClientHeartbeat(string contextConnectionId);
    }
}
