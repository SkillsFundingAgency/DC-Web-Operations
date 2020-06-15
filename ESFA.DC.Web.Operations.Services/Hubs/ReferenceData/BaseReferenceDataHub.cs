using System;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.PeriodEnd;
using Microsoft.AspNetCore.SignalR;

namespace ESFA.DC.Web.Operations.Services.Hubs.ReferenceData
{
    public class BaseReferenceDataHub : Hub
    {
        private readonly ISerialisationHelperService _serialisationHelperService;
        private readonly IHubEventBase _eventBase;
        private readonly ILogger _logger;

        public BaseReferenceDataHub(
            ISerialisationHelperService serialisationHelperService,
            IHubEventBase eventBase,
            ILogger logger)
        {
            _serialisationHelperService = serialisationHelperService;
            _eventBase = eventBase;
            _logger = logger;
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

        public virtual string SerialiseToCamelCase<T>(T model)
        {
            return _serialisationHelperService.SerialiseToCamelCase(model);
        }
    }
}