using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.PeriodEnd;
using ESFA.DC.Web.Operations.Interfaces.ReferenceData;

namespace ESFA.DC.Web.Operations.Services.TimedHostedService.ReferenceData
{
    public class ReferenceDataTimedHostedService : BaseTimedHostedService
    {
        private readonly ILogger _logger;
        private readonly IEnumerable<IReferenceDataHub> _hubs;

        public ReferenceDataTimedHostedService(
            ILogger logger,
            IHubEventBase eventBase,
            ISerialisationHelperService serialisationHelperService,
            IEnumerable<IReferenceDataHub> hubs)
            : base("Reference Data", logger, serialisationHelperService)
        {
            _logger = logger;
            _hubs = hubs;
            eventBase.HubCallback += RegisterClient;
        }

        protected override async Task DoWork(CancellationToken cancellationToken)
        {
            try
            {
                foreach (var hub in _hubs)
                {
                    await hub.SendMessage(cancellationToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to {nameof(DoWork)} in {nameof(ReferenceDataTimedHostedService)}", ex);
            }
        }
    }
}