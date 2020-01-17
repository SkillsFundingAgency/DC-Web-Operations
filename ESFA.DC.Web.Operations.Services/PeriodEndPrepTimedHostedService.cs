using System;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.PeriodEnd.Models;
using ESFA.DC.Web.Operations.Interfaces.PeriodEnd;
using ESFA.DC.Web.Operations.Services.Hubs;

namespace ESFA.DC.Web.Operations.Services
{
    public sealed class PeriodEndPrepTimedHostedService : BaseTimedHostedService
    {
        private readonly IPeriodService _periodService;
        private readonly ILogger _logger;
        private readonly IPeriodEndService _periodEndService;
        private readonly PeriodEndPrepHub _periodEndPrepHub;

        public PeriodEndPrepTimedHostedService(
            IPeriodService periodService,
            ILogger logger,
            IPeriodEndService periodEndService,
            IPeriodEndPrepHubEventBase eventBase,
            PeriodEndPrepHub periodEndPrepHub)
        : base("Period End Prep", logger)
        {
            _periodService = periodService;
            _logger = logger;
            _periodEndService = periodEndService;
            _periodEndPrepHub = periodEndPrepHub;
            eventBase.PeriodEndHubPrepCallback += RegisterClient;
        }

        protected override async Task DoWork(CancellationToken cancellationToken)
        {
            try
            {
                PathYearPeriod currentPeriod = await _periodService.ReturnPeriod(cancellationToken);
                if (currentPeriod.Year == null)
                {
                    return;
                }

                // Get state JSON.
                Task<string> pathItemStates = _periodEndService.GetPathItemStates(
                    currentPeriod.Year.Value,
                    currentPeriod.Period,
                    cancellationToken);
                Task<string> failedJobs = _periodEndService.GetFailedJobs(
                    currentPeriod.Year.Value,
                    currentPeriod.Period,
                    cancellationToken);
                Task<string> referenceDataJobs = _periodEndService.GetReferenceDataJobs(cancellationToken);

                await Task.WhenAll(pathItemStates, failedJobs, referenceDataJobs);

                // Send JSON to clients.
                await _periodEndPrepHub.SendMessage(referenceDataJobs.Result, failedJobs.Result, pathItemStates.Result, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to {nameof(DoWork)} in {nameof(PeriodEndPrepTimedHostedService)}", ex);
            }
        }
    }
}