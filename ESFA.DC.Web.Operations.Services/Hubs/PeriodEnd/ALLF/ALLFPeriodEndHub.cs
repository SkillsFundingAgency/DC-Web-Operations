﻿using System;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.PeriodEnd;
using ESFA.DC.Web.Operations.Utils;
using Microsoft.AspNetCore.SignalR;

namespace ESFA.DC.Web.Operations.Services.Hubs.PeriodEnd.ALLF
{
    public class ALLFPeriodEndHub : Hub
    {
        private const string CollectionType = CollectionTypes.ALLF;
        private readonly IHubEventBase _eventBase;
        private readonly IHubContext<ALLFPeriodEndHub> _hubContext;
        private readonly IALLFPeriodEndService _periodEndService;
        private readonly IStateService _stateService;
        private readonly IPeriodService _periodService;
        private readonly ILogger _logger;

        public ALLFPeriodEndHub(
            IHubEventBase eventBase,
            IHubContext<ALLFPeriodEndHub> hubContext,
            IALLFPeriodEndService periodEndService,
            IStateService stateService,
            IPeriodService periodService,
            ILogger logger)
        {
            _eventBase = eventBase;
            _hubContext = hubContext;
            _periodEndService = periodEndService;
            _stateService = stateService;
            _periodService = periodService;
            _logger = logger;
        }

        public async Task SendMessage(string paths, CancellationToken cancellationToken)
        {
            await SetButtonStates(cancellationToken);

            await _hubContext.Clients.All.SendAsync("ReceiveMessage", paths, cancellationToken);
        }

        public async Task ReceiveMessage()
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

        public async Task StartPeriodEnd(int collectionYear, int period)
        {
            try
            {
                await _periodEndService.StartPeriodEndAsync(collectionYear, period, CollectionType, CancellationToken.None);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                throw;
            }
        }

        public async Task ClosePeriodEnd(int collectionYear, int period)
        {
            try
            {
                await _hubContext.Clients.All.SendAsync("TurnOffMessage");
                await _periodEndService.ClosePeriodEndAsync(collectionYear, period, CollectionType, CancellationToken.None);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                throw;
            }
        }

        public async Task Proceed(int collectionYear, int period, int pathId, int pathItemId)
        {
            try
            {
                await _hubContext.Clients.All.SendAsync("DisablePathItemProceed", pathItemId);
                await _periodEndService.ProceedAsync(collectionYear, period, pathId, CancellationToken.None);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                throw;
            }
        }

        public async Task ReSubmitJob(int jobId)
        {
            try
            {
                await _hubContext.Clients.All.SendAsync("DisableJobReSubmit", jobId);
                await _periodEndService.ReSubmitFailedJobAsync(jobId, CancellationToken.None);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                throw;
            }
        }

        private async Task SetButtonStates(CancellationToken cancellationToken)
        {
            var period = await _periodService.ReturnPeriod(CollectionType, cancellationToken);
            var periodClosed = period.PeriodClosed;

            string stateString = await _periodEndService.GetPathItemStatesAsync(period.Year ?? 0, period.Period, CollectionType, cancellationToken);
            var state = _stateService.GetMainState(stateString);

            var startEnabled = periodClosed && !state.PeriodEndStarted && !state.PeriodEndFinished;

            if (PeriodEndState.CurrentAction != Constants.Action_UploadFileButton)
            {
                await _hubContext.Clients.All.SendAsync(Constants.Action_UploadFileButton, !state.PeriodEndFinished, cancellationToken);
            }

            if (PeriodEndState.CurrentAction != Constants.Action_StartPeriodEndButton)
            {
                await _hubContext.Clients.All.SendAsync(Constants.Action_StartPeriodEndButton, startEnabled, cancellationToken);
            }

            if (PeriodEndState.CurrentAction != Constants.Action_PeriodClosedButton)
            {
                var closeEnabled = periodClosed && !state.PeriodEndFinished && _stateService.AllJobsHaveCompleted(state);
                await _hubContext.Clients.All.SendAsync(Constants.Action_PeriodClosedButton, closeEnabled, cancellationToken);
            }
        }
    }
}