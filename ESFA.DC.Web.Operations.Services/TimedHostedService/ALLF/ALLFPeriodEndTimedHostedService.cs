using System;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.PeriodEnd;
using ESFA.DC.Web.Operations.Services.Hubs.PeriodEnd.ALLF;
using ESFA.DC.Web.Operations.Utils;

namespace ESFA.DC.Web.Operations.Services.TimedHostedService.ALLF
{
    public class ALLFPeriodEndTimedHostedService : BaseTimedHostedService
    {
        private readonly ILogger _logger;
        private readonly IALLFPeriodEndService _periodEndService;
        private readonly ALLFPeriodEndHub _periodEndHub;

        public ALLFPeriodEndTimedHostedService(
            ILogger logger,
            IALLFPeriodEndService periodEndService,
            IPeriodEndHubEventBase eventBase,
            ISerialisationHelperService serialisationHelperService,
            ALLFPeriodEndHub periodEndHub)
            : base("ALLF Period End", logger, serialisationHelperService)
        {
            _logger = logger;
            _periodEndService = periodEndService;
            _periodEndHub = periodEndHub;
            eventBase.PeriodEndHubCallback += RegisterClient;
        }

        protected override async Task DoWork(CancellationToken cancellationToken)
        {
            try
            {
                // Get state JSON.
                var model = await _periodEndService.GetPathState(null, null, cancellationToken);

                var state = SerialiseToCamelCase(model);

                // Send JSON to clients.
                await _periodEndHub.SendMessage(state, CollectionTypes.ALLF, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to {nameof(DoWork)} in {nameof(ALLFPeriodEndTimedHostedService)}", ex);
            }
        }
    }
}