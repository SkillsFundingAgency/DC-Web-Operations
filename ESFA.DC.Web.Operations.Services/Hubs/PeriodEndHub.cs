using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace ESFA.DC.Web.Operations.Services.Hubs
{
    public class PeriodEndHub : Hub
    {
        private readonly IHubContext<PeriodEndHub> _hubContext;

        public PeriodEndHub(IHubContext<PeriodEndHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SendMessage(string paths, CancellationToken cancellationToken)
        {
            await _hubContext.Clients.All.SendAsync("ReceiveMessage", paths, cancellationToken);
        }
    }
}