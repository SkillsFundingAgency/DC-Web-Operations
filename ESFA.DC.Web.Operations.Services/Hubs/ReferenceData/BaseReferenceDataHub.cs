using System;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Interfaces;
using ESFA.DC.Web.Operations.Services.HubUserHandlers;
using ESFA.DC.Web.Operations.Utils;
using Microsoft.AspNetCore.SignalR;

namespace ESFA.DC.Web.Operations.Services.Hubs.ReferenceData
{
    public class BaseReferenceDataHub : Hub
    {
        private readonly IHubEventBase _eventBase;
        private readonly ILogger _logger;
        private readonly ReferenceDataTypes _referenceDataType;

        public BaseReferenceDataHub(
            IHubEventBase eventBase,
            ILogger logger,
            ReferenceDataTypes referenceDataType)
        {
            _eventBase = eventBase;
            _logger = logger;
            _referenceDataType = referenceDataType;
        }

        public void ReceiveMessage()
        {
            try
            {
                _eventBase.TriggerHub(Context.ConnectionId);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                throw;
            }
        }

        public override Task OnConnectedAsync()
        {
            ReferenceDataHubUserHandler.AddConnectionId(_referenceDataType, Context.ConnectionId);
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            ReferenceDataHubUserHandler.RemoveConnectionId(_referenceDataType, Context.ConnectionId);
            return base.OnDisconnectedAsync(exception);
        }
    }
}