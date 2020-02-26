using System;

namespace ESFA.DC.Web.Operations.Interfaces.Processing
{
    public interface IJobFailedTodayHubEventBase
    {
        event EventHandler ClientHeartbeatCallback;

        void ClientHeartbeat(string contextConnectionId);
    }
}
