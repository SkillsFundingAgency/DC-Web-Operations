using System;

namespace ESFA.DC.Web.Operations.Interfaces.Dashboard
{
    public interface IDashBoardHubEventBase
    {
        event EventHandler ClientHeartbeatCallback;

        void ClientHeartbeat(string contextConnectionId);
    }
}
