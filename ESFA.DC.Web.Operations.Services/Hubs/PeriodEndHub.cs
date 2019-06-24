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

        public async Task SendMessage(string paths)
        {
            await _hubContext.Clients.All.SendAsync("ReceiveMessage", paths);

            //await Clients.All.SendAsync("ReceiveMessage", paths);
        }
    }
}