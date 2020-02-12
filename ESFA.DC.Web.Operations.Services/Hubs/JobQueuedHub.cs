using System;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.Processing;
using Microsoft.AspNetCore.SignalR;

namespace ESFA.DC.Web.Operations.Services.Hubs
{
    public class JobQueuedHub : Hub
    {
        private readonly IJobQueuedHubEventBase _eventBase;
        private readonly IHubContext<JobQueuedHub> _hubContext;
        private readonly ILogger _logger;

        public JobQueuedHub(IJobQueuedHubEventBase eventBase, IHubContext<JobQueuedHub> hubContext, ILogger logger)
        {
            _eventBase = eventBase;
            _hubContext = hubContext;
            _logger = logger;
        }

        public async Task ReceiveMessage()
        {
            try
            {
                _eventBase.ClientHeartbeat(Context.ConnectionId);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                throw;
            }
        }

        public async Task SendMessage(string jobQueuedModel)
        {
            await _hubContext.Clients.All.SendAsync("ReceiveMessage", jobQueuedModel);
        }
    }
}
