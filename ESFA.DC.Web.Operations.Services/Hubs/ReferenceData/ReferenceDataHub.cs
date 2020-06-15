using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace ESFA.DC.Web.Operations.Services.Hubs.ReferenceData
{
    public class ReferenceDataHub : Hub
    {
        private readonly IHubContext<ReferenceDataHub> _hubContext;
        private readonly ILogger _logger;

        public ReferenceDataHub(
            IHubContext<ReferenceDataHub> hubContext,
            ILogger logger)
        {
            _hubContext = hubContext;
            _logger = logger;
        }

        public async Task SendMessage(string paths, CancellationToken cancellationToken)
        {
            await _hubContext.Clients.All.SendAsync("ReceiveMessage", paths, cancellationToken);
        }
    }
}