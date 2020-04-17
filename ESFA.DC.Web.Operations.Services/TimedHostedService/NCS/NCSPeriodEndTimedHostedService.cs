﻿using System;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.PeriodEnd;
using ESFA.DC.Web.Operations.Services.Hubs.PeriodEnd.NCS;
using ESFA.DC.Web.Operations.Utils;

namespace ESFA.DC.Web.Operations.Services.TimedHostedService.NCS
{
    public class NCSPeriodEndTimedHostedService : BaseTimedHostedService
    {
        private readonly ILogger _logger;
        private readonly IPeriodEndService _periodEndService;
        private readonly NCSPeriodEndHub _periodEndHub;

        public NCSPeriodEndTimedHostedService(
            ILogger logger,
            IPeriodEndService periodEndService,
            IPeriodEndHubEventBase eventBase,
            NCSPeriodEndHub periodEndHub)
            : base("NCS Period End", logger)
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
                string pathItemStates = await _periodEndService.GetPathItemStatesAsync(null, null, CollectionTypes.ILR, cancellationToken);

                // Send JSON to clients.
                await _periodEndHub.SendMessage(pathItemStates, CollectionTypes.ILR, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to {nameof(DoWork)} in {nameof(NCSPeriodEndTimedHostedService)}", ex);
            }
        }
    }
}