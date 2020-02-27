using System;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.Processing;
using Microsoft.AspNetCore.SignalR;

namespace ESFA.DC.Web.Operations.Services.Hubs
{
    public abstract class BaseHub<THub> : Hub
        where THub : Hub
    {
        private readonly IHubEventBase _eventBase;
        private readonly IHubContext<THub> _hubContext;
        private readonly ILogger _logger;

        public BaseHub(IHubEventBase eventBase, IHubContext<THub> hubContext, ILogger logger)
        {
            _eventBase = eventBase;
            _hubContext = hubContext;
            _logger = logger;
        }

        public virtual async Task ReceiveMessage()
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

        public virtual async Task SendMessage(string jobModel)
        {
            await _hubContext.Clients.All.SendAsync("ReceiveMessage", jobModel);
        }
    }
}
