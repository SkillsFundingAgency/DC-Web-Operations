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

        public PeriodEndHub(
            IHubEventBase eventBase,
            IHubContext<PeriodEndHub> hubContext)
        {
            _eventBase = eventBase;
            _hubContext = hubContext;
        }

        public async Task SendMessage(string paths, CancellationToken cancellationToken)
        {
            await _hubContext.Clients.All.SendAsync("ReceiveMessage", paths, cancellationToken);
        }

        public async Task ReceiveMessage()
        {
            _eventBase.TriggerPeriodEnd();
        }
    }
}