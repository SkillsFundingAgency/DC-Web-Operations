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

        public PeriodEndHub(
            IHubEventBase eventBase,
            IHubContext<PeriodEndHub> hubContext,
            IPeriodEndService periodEndService,
            IEmailService emailService)
        {
            _eventBase = eventBase;
            _hubContext = hubContext;
            _periodEndService = periodEndService;
            _emailService = emailService;
        }

        public async Task SendMessage(string paths, CancellationToken cancellationToken)
        {
            await _hubContext.Clients.All.SendAsync("ReceiveMessage", paths, cancellationToken);
        }

        public void ReceiveMessage()
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