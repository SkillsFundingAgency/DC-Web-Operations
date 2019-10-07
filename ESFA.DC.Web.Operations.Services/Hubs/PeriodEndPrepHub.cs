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

        public PeriodEndPrepHub(
            IHubEventBase eventBase,
            IHubContext<PeriodEndPrepHub> hubContext,
            IPeriodEndService periodEndService,
            IEmailService emailService)
        {
            _eventBase = eventBase;
            _hubContext = hubContext;
            _periodEndService = periodEndService;
            _emailService = emailService;
        }

        public async Task SendMessage(string referenceJobs, string failedJobs, CancellationToken cancellationToken)
        {
            await _hubContext.Clients.All.SendAsync("ReceiveMessage", referenceJobs, failedJobs, cancellationToken);
        }

        public async Task ReceiveMessage()
        {
            _eventBase.TriggerPeriodEndPrep();
        }

        public async Task SendCollectionClosedEmail(int period)
        {
            await _emailService.SendEmail(EmailIds.ConfirmCollectionClosedEmail, period);
        }

        public async Task PauseReferenceDataJobs()
        {
            await _periodEndService.ToggleReferenceDataJobs(true);
        }

        public async Task ReSubmitJob(int jobId)
        {
            await _hubContext.Clients.All.SendAsync("DisableJobReSubmit", jobId);
            await _periodEndService.ReSubmitFailedJob(jobId);
        }
    }
}