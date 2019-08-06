using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace ESFA.DC.Web.Operations.Services.Hubs
{
    public class PeriodEndPrepHub : Hub
    {
        private readonly IHubContext<PeriodEndPrepHub> _hubContext;

        public PeriodEndPrepHub(IHubContext<PeriodEndPrepHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SendMessage(string paths, CancellationToken cancellationToken)
        {
            await _hubContext.Clients.All.SendAsync("ReceiveMessage", paths, cancellationToken);
        }
    }
}