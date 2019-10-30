using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Web.Operations.Interfaces.PeriodEnd;
using ESFA.DC.Web.Operations.Utils;
using Microsoft.AspNetCore.SignalR;

namespace ESFA.DC.Web.Operations.Services.Hubs
{
    public class PeriodEndHub : Hub
    {
        private readonly IHubEventBase _eventBase;
        private readonly IHubContext<PeriodEndHub> _hubContext;
        private readonly IPeriodEndService _periodEndService;
        private readonly IEmailService _emailService;
        private readonly IStateService _stateService;
        private readonly IPeriodService _periodService;

        public PeriodEndHub(
            IHubEventBase eventBase,
            IHubContext<PeriodEndHub> hubContext,
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

        public async Task SendMessage(string paths, CancellationToken cancellationToken)
        {
            await SetButtonStates(paths, cancellationToken);

            await _hubContext.Clients.All.SendAsync("ReceiveMessage", paths, cancellationToken);
        }

        public async Task SetButtonStates(string paths, CancellationToken cancellationToken)
        {
            var period = await _periodService.ReturnPeriod(cancellationToken);
            var periodClosed = period.PeriodClosed;

            string state = await _periodEndService.GetPathItemStates(period.Year.Value, period.Period, cancellationToken);
            var periodEndState = _stateService.GetPeriodEndState(state);

            var startEnabled = periodClosed && !periodEndState.PeriodEndStarted;
            if (PeriodEndState.CurrentAction != Constants.Action_StartPeriodEndButton)
            {
                await _hubContext.Clients.All.SendAsync(Constants.Action_StartPeriodEndButton, startEnabled, cancellationToken);
            }

            if (PeriodEndState.CurrentAction != Constants.Action_MCAReportsButton)
            {
                var mcaEnabled = periodClosed && !startEnabled && !periodEndState.McaReportsPublished && periodEndState.McaReportsReady;
                await _hubContext.Clients.All.SendAsync(Constants.Action_MCAReportsButton, mcaEnabled, cancellationToken);
            }

            if (PeriodEndState.CurrentAction != Constants.Action_ProviderReportsButton)
            {
                var providerEnabled = periodClosed && !periodEndState.ProviderReportsPublished && periodEndState.ProviderReportsReady;
                await _hubContext.Clients.All.SendAsync(Constants.Action_ProviderReportsButton, providerEnabled, cancellationToken);
            }

            if (PeriodEndState.CurrentAction != Constants.Action_PeriodClosedButton)
            {
                var closeEnabled = periodClosed && !periodEndState.PeriodEndClosed && periodEndState.McaReportsPublished && periodEndState.ProviderReportsPublished;
                await _hubContext.Clients.All.SendAsync(Constants.Action_PeriodClosedButton, closeEnabled, cancellationToken);
            }

            if (PeriodEndState.CurrentAction != Constants.Action_ReferenceJobsButton)
            {
                var referenceJobsUnPausedEnabled = periodClosed && periodEndState.PeriodEndClosed && periodEndState.ReferenceDataJobsPaused;
                await _hubContext.Clients.All.SendAsync(Constants.Action_ReferenceJobsButton, referenceJobsUnPausedEnabled, cancellationToken);
            }
        }

        public async Task ReceiveMessage()
        {
            _eventBase.TriggerPeriodEnd();
        }

        public async Task StartPeriodEnd(int collectionYear, int period)
        {
            await _hubContext.Clients.All.SendAsync("DisableStartPeriodEnd");
            await _periodEndService.StartPeriodEnd(collectionYear, period);

            await _emailService.SendEmail(EmailIds.PeriodEndStartedEmail, period);
        }

        public async Task Proceed(int collectionYear, int period, int pathId, int pathItemId)
        {
            await _hubContext.Clients.All.SendAsync("DisablePathItemProceed", pathItemId);
            await _periodEndService.Proceed(collectionYear, period, pathId);
        }
    }
}