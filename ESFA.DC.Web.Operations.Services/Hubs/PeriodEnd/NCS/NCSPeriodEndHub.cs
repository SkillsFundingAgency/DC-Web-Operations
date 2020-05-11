using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.PeriodEnd;
using ESFA.DC.Web.Operations.Utils;
using Microsoft.AspNetCore.SignalR;

namespace ESFA.DC.Web.Operations.Services.Hubs.PeriodEnd.NCS
{
    public class NCSPeriodEndHub : Hub
    {
        private readonly IPeriodEndHubEventBase _eventBase;
        private readonly IHubContext<NCSPeriodEndHub> _hubContext;
        private readonly INCSPeriodEndService _periodEndService;
        private readonly IEmailService _emailService;
        private readonly IStateService _stateService;
        private readonly IPeriodService _periodService;
        private readonly ILogger _logger;

        public NCSPeriodEndHub(
            IPeriodEndHubEventBase eventBase,
            IHubContext<NCSPeriodEndHub> hubContext,
            INCSPeriodEndService periodEndService,
            IEmailService emailService,
            IStateService stateService,
            IPeriodService periodService,
            ILogger logger)
        {
            _eventBase = eventBase;
            _hubContext = hubContext;
            _periodEndService = periodEndService;
            _emailService = emailService;
            _stateService = stateService;
            _periodService = periodService;
            _logger = logger;
        }

        public async Task SendMessage(string paths, string collectionType, CancellationToken cancellationToken)
        {
            await SetButtonStates(collectionType, cancellationToken);

            await _hubContext.Clients.All.SendAsync("ReceiveMessage", paths, cancellationToken);
        }

        public async Task ReceiveMessage()
        {
            try
            {
                _eventBase.TriggerPeriodEnd(Context.ConnectionId);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                throw;
            }
        }

        public async Task StartPeriodEnd(int collectionYear, int period, string collectionType)
        {
            try
            {
                await _periodEndService.StartPeriodEndAsync(collectionYear, period, collectionType);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                throw;
            }
        }

        public async Task ClosePeriodEnd(int collectionYear, int period, string collectionType)
        {
            try
            {
                await _hubContext.Clients.All.SendAsync("TurnOffMessage");
                await _periodEndService.ClosePeriodEndAsync(collectionYear, period, collectionType);
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
                await _periodEndService.ProceedAsync(collectionYear, period, pathId);
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
                await _periodEndService.ReSubmitFailedJobAsync(jobId);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                throw;
            }
        }

        private async Task SetButtonStates(string collectionType, CancellationToken cancellationToken)
        {
            var period = await _periodService.ReturnPeriod(collectionType, cancellationToken);
            var periodClosed = period.PeriodClosed;

            string stateString = await _periodEndService.GetPathItemStatesAsync(period.Year.Value, period.Period, collectionType, cancellationToken);
            var state = _stateService.GetMainState(stateString);

            var startEnabled = periodClosed && !state.PeriodEndStarted && !state.PeriodEndFinished;
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