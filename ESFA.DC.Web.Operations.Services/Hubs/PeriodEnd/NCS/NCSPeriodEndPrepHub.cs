using System;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.PeriodEnd;
using ESFA.DC.Web.Operations.Utils;
using Microsoft.AspNetCore.SignalR;

namespace ESFA.DC.Web.Operations.Services.Hubs.PeriodEnd.NCS
{
    public class NCSPeriodEndPrepHub : Hub
    {
        private readonly IPeriodEndPrepHubEventBase _eventBase;
        private readonly IHubContext<NCSPeriodEndPrepHub> _hubContext;
        private readonly INCSPeriodEndService _periodEndService;
        private readonly IEmailService _emailService;
        private readonly IStateService _stateService;
        private readonly IPeriodService _periodService;
        private readonly ILogger _logger;

        public NCSPeriodEndPrepHub(
            IPeriodEndPrepHubEventBase eventBase,
            IHubContext<NCSPeriodEndPrepHub> hubContext,
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

        public async Task SendMessage(
            string state,
            CancellationToken cancellationToken)
        {
            await SetButtonStates(state, cancellationToken);

            await _hubContext.Clients.All.SendAsync("ReceiveMessage", state, cancellationToken);
        }

        public async Task SetButtonStates(string state, CancellationToken cancellationToken)
        {
            var prepModel = _stateService.GetPrepState(state);
            var period = await _periodService.ReturnPeriod(CollectionTypes.NCS, cancellationToken);
            var collectionClosedEmailEnabled = period.PeriodClosed && !_stateService.CollectionClosedEmailSent(prepModel.State);
            var continueEnabled = !collectionClosedEmailEnabled && period.PeriodClosed;

            if (PeriodEndState.CurrentAction != Constants.Action_CollectionClosedEmailButton)
            {
                await _hubContext.Clients.All.SendAsync(Constants.Action_CollectionClosedEmailButton, collectionClosedEmailEnabled, cancellationToken);
            }

            if (PeriodEndState.CurrentAction != Constants.Action_ContinueButton)
            {
                await _hubContext.Clients.All.SendAsync(Constants.Action_ContinueButton, continueEnabled, cancellationToken);
            }
        }

        public async Task ReceiveMessage()
        {
            try
            {
                _eventBase.TriggerPeriodEndPrep(Context.ConnectionId);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                throw;
            }
        }

        public async Task SendCollectionClosedEmail(int year, int period, string collectionType)
        {
            try
            {
                PeriodEndState.CurrentAction = Constants.Action_CollectionClosedEmailButton;
                await _hubContext.Clients.All.SendAsync(Constants.Action_CollectionClosedEmailButton, false);

                await _periodEndService.InitialisePeriodEndAsync(year, period, collectionType, CancellationToken.None);
                await _periodEndService.CollectionClosedEmailSentAsync(year, period);
                await _emailService.SendEmail(EmailIds.ConfirmCollectionClosedEmail, period);
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
    }
}