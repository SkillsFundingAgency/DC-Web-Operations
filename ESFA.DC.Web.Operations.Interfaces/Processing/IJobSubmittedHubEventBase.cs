using System;

namespace ESFA.DC.Web.Operations.Interfaces.Processing
{
    public interface IJobSubmittedHubEventBase
    {
        event EventHandler ClientHeartbeatCallback;

        void ClientHeartbeat(string contextConnectionId);
    }
}
