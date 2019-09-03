using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Web.Operations.Interfaces.PeriodEnd;
using Microsoft.AspNetCore.SignalR;

namespace ESFA.DC.Web.Operations.Services.Hubs
{
    public class PeriodEndHub : Hub
    {
        private readonly IHubEventBase _eventBase;
        private readonly IHubContext<PeriodEndHub> _hubContext;
        private readonly IPeriodEndService _periodEndService;

        public PeriodEndHub(
            IHubEventBase eventBase,
            IHubContext<PeriodEndHub> hubContext,
            IPeriodEndService periodEndService)
        {
            _eventBase = eventBase;
            _hubContext = hubContext;
            _periodEndService = periodEndService;
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
        }
    }
}