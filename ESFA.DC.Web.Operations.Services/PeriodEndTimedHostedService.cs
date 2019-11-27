using System;
using System.Threading;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.PeriodEnd.Models;
using ESFA.DC.Web.Operations.Interfaces.PeriodEnd;
using ESFA.DC.Web.Operations.Services.Hubs;

namespace ESFA.DC.Web.Operations.Services
{
    public sealed class PeriodEndTimedHostedService : BaseTimedHostedService
    {
        private readonly IPeriodService _periodService;
        private readonly ILogger _logger;
        private readonly IPeriodEndService _periodEndService;
        private readonly PeriodEndHub _periodEndHub;
        private readonly PeriodEndPrepHub _periodEndPrepHub;

        public PeriodEndTimedHostedService(
            IPeriodService periodService,
            ILogger logger,
            IPeriodEndService periodEndService,
            IHubEventBase eventBase,
            PeriodEndHub periodEndHub,
            PeriodEndPrepHub periodEndPrepHub)
        : base("Period End", logger)
        {
            _periodService = periodService;
            _logger = logger;
            _periodEndService = periodEndService;
            _periodEndHub = periodEndHub;
            _periodEndPrepHub = periodEndPrepHub;
            eventBase.PeriodEndHubCallback += RegisterClient;
            eventBase.PeriodEndHubPrepCallback += RegisterClient;
        }

        protected override async void DoWork(CancellationToken cancellationToken)
        {
            try
            {
                PathYearPeriod currentPeriod = await _periodService.ReturnPeriod(cancellationToken);
                if (currentPeriod.Year == null)
                {
                    return;
                }

                // Get state JSON.
                string pathItemStates = await _periodEndService.GetPathItemStates(
                    currentPeriod.Year.Value,
                    currentPeriod.Period,
                    cancellationToken);
                string failedJobs = await _periodEndService.GetFailedJobs(
                    currentPeriod.Year.Value,
                    currentPeriod.Period,
                    cancellationToken);
                string referenceDataJobs = await _periodEndService.GetReferenceDataJobs(cancellationToken);

                // Send JSON to clients.
                await _periodEndHub.SendMessage(pathItemStates, cancellationToken);
                await _periodEndPrepHub.SendMessage(referenceDataJobs, failedJobs, pathItemStates, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to {nameof(DoWork)} in {nameof(PeriodEndTimedHostedService)}", ex);
            }
        }
    }
}