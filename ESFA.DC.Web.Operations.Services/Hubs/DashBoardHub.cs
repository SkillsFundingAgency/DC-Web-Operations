using System;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.Dashboard;
using Microsoft.AspNetCore.SignalR;

namespace ESFA.DC.Web.Operations.Services.Hubs
{
    public sealed class DashBoardHub : Hub
    {
        private readonly IDashBoardHubEventBase _eventBase;
        private readonly IHubContext<DashBoardHub> _hubContext;
        private readonly ILogger _logger;

        public DashBoardHub(
            IDashBoardHubEventBase eventBase,
            IHubContext<DashBoardHub> hubContext,
            ILogger logger)
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

        public async Task SendMessage(string dashBoardModel)
        {
            await _hubContext.Clients.All.SendAsync("ReceiveMessage", dashBoardModel);
        }
    }
}
