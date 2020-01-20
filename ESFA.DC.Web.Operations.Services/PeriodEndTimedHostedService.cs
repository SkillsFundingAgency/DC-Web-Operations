using System;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.PeriodEnd;
using ESFA.DC.Web.Operations.Services.Hubs;

namespace ESFA.DC.Web.Operations.Services
{
    public sealed class PeriodEndTimedHostedService : BaseTimedHostedService
    {
        private readonly ILogger _logger;
        private readonly IPeriodEndService _periodEndService;
        private readonly PeriodEndHub _periodEndHub;

        public PeriodEndTimedHostedService(
            ILogger logger,
            IPeriodEndService periodEndService,
            IPeriodEndHubEventBase eventBase,
            PeriodEndHub periodEndHub)
        : base("Period End", logger)
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
                string pathItemStates = await _periodEndService.GetPathItemStates(null, null, cancellationToken);

                // Send JSON to clients.
                await _periodEndHub.SendMessage(pathItemStates, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to {nameof(DoWork)} in {nameof(PeriodEndTimedHostedService)}", ex);
            }
        }
    }
}