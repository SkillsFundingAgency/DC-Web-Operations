using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Web.Operations.Interfaces.PeriodEnd;
using ESFA.DC.Web.Operations.Utils;
using Microsoft.AspNetCore.SignalR;

namespace ESFA.DC.Web.Operations.Services.Hubs
{
    public class PeriodEndPrepHub : Hub
    {
        private readonly IHubEventBase _eventBase;
        private readonly IHubContext<PeriodEndPrepHub> _hubContext;
        private readonly IPeriodEndService _periodEndService;
        private readonly IEmailService _emailService;
        private readonly IStateService _stateService;
        private readonly IPeriodService _periodService;

        public PeriodEndPrepHub(
            IHubEventBase eventBase,
            IHubContext<PeriodEndPrepHub> hubContext,
            IPeriodEndService periodEndService,
            IEmailService emailService,
            IStateService stateService,
            IPeriodService periodService)
        {
            _eventBase = eventBase;
            _hubContext = hubContext;
            _periodEndService = periodEndService;
            _emailService = emailService;
            _stateService = stateService;
            _periodService = periodService;
        }

        public async Task SendMessage(string referenceJobs, string failedJobs, string pathStates, CancellationToken cancellationToken)
        {
            await SetButtonStates(referenceJobs, pathStates, cancellationToken);

            await _hubContext.Clients.All.SendAsync("ReceiveMessage", referenceJobs, failedJobs, cancellationToken);
        }

        public async Task SetButtonStates(string referenceJobs, string pathStates, CancellationToken cancellationToken)
        {
            var pauseEnabled = _stateService.PauseReferenceDataIsEnabled(referenceJobs);
            var period = await _periodService.ReturnPeriod(cancellationToken);
            var collectionClosedEmailEnabled = period.PeriodClosed && !pauseEnabled && !_stateService.CollectionClosedEmailSent(pathStates);
            var continueEnabled = !pauseEnabled && !collectionClosedEmailEnabled && period.PeriodClosed;

            if (PeriodEndState.CurrentAction != Constants.Action_ReferenceJobsButton)
            {
                await _hubContext.Clients.All.SendAsync(Constants.Action_ReferenceJobsButton, pauseEnabled, cancellationToken);
            }

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
            _eventBase.TriggerPeriodEndPrep();
        }

        public async Task SendCollectionClosedEmail(int year, int period)
        {
            PeriodEndState.CurrentAction = Constants.Action_CollectionClosedEmailButton;
            await _hubContext.Clients.All.SendAsync(Constants.Action_CollectionClosedEmailButton, false);

            await _periodEndService.CollectionClosedEmailSent(year, period);
            await _emailService.SendEmail(EmailIds.ConfirmCollectionClosedEmail, period);
        }

        public async Task PauseReferenceDataJobs(int year, int period)
        {
            PeriodEndState.CurrentAction = Constants.Action_ReferenceJobsButton;
            await _hubContext.Clients.All.SendAsync(Constants.Action_ReferenceJobsButton, false);

            await _periodEndService.InitialisePeriodEnd(year, period);
            await _periodEndService.ToggleReferenceDataJobs(true);
        }

        public async Task ReSubmitJob(int jobId)
        {
            await _hubContext.Clients.All.SendAsync("DisableJobReSubmit", jobId);
            await _periodEndService.ReSubmitFailedJob(jobId);
        }
    }
}