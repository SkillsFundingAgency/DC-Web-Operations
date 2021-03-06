﻿using System;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.PeriodEnd;
using ESFA.DC.Web.Operations.Services.Hubs.PeriodEnd.NCS;
using ESFA.DC.Web.Operations.Utils;

namespace ESFA.DC.Web.Operations.Services.TimedHostedService.NCS
{
    public class NCSPeriodEndPrepTimedHostedService : BaseTimedHostedService
    {
        private readonly ILogger _logger;
        private readonly INCSPeriodEndService _periodEndService;
        private readonly NCSPeriodEndPrepHub _periodEndPrepHub;

        public NCSPeriodEndPrepTimedHostedService(
            ILogger logger,
            INCSPeriodEndService periodEndService,
            IPeriodEndPrepHubEventBase eventBase,
            ISerialisationHelperService serialisationHelperService,
            NCSPeriodEndPrepHub periodEndPrepHub)
            : base("NCS Period End Prep", logger, serialisationHelperService)
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
                string state = await _periodEndService.GetPrepStateAsync(null, null, cancellationToken);

                // Send JSON to clients.
                await _periodEndPrepHub.SendMessage(state, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to {nameof(DoWork)} in {nameof(NCSPeriodEndPrepTimedHostedService)}", ex);
            }
        }
    }
}