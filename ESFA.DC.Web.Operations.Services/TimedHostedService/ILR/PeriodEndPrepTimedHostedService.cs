using System;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.PeriodEnd;
using ESFA.DC.Web.Operations.Services.Hubs;
using ESFA.DC.Web.Operations.Utils;

namespace ESFA.DC.Web.Operations.Services.TimedHostedService.ILR
{
    public sealed class PeriodEndPrepTimedHostedService : BaseTimedHostedService
    {
        private readonly ILogger _logger;
        private readonly IPeriodEndService _periodEndService;
        private readonly PeriodEndPrepHub _periodEndPrepHub;

        public PeriodEndPrepTimedHostedService(
            ILogger logger,
            IPeriodEndService periodEndService,
            IPeriodEndPrepHubEventBase eventBase,
            PeriodEndPrepHub periodEndPrepHub)
        : base("Period End Prep", logger)
        {
            _logger = logger;
            _periodEndService = periodEndService;
            _periodEndPrepHub = periodEndPrepHub;
            eventBase.PeriodEndHubPrepCallback += RegisterClient;
        }

        protected override async Task DoWork(CancellationToken cancellationToken)
        {
            try
            {
                string state = await _periodEndService.GetPrepState(null, null, CollectionTypes.ILR, cancellationToken);

                // Send JSON to clients.
                await _periodEndPrepHub.SendMessage(state, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to {nameof(DoWork)} in {nameof(PeriodEndPrepTimedHostedService)}", ex);
            }
        }
    }
}