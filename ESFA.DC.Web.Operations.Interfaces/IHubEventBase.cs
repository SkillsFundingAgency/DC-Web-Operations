using System;

namespace ESFA.DC.Web.Operations.Interfaces
{
    public interface IHubEventBase
    {
        event EventHandler HubCallback;

        void TriggerHub(string contextConnectionId);
    }
}