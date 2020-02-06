using System;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.Processing;
using ESFA.DC.Web.Operations.Models.JobsProcessing;
using Microsoft.AspNetCore.SignalR;

namespace ESFA.DC.Web.Operations.Services.Hubs
{
    public class JobProcessingHub : Hub
    {
        private readonly IJobProcessingHubEventBase _eventBase;
        private readonly IHubContext<JobProcessingHub> _hubContext;
        private readonly ILogger _logger;

        public JobProcessingHub(IJobProcessingHubEventBase eventBase, IHubContext<JobProcessingHub> hubContext, ILogger logger)
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

        public async Task SendMessage(JobsProcessingModel jobProcessingModel)
        {
            await _hubContext.Clients.All.SendAsync("ReceiveJobs", jobProcessingModel);
        }
    }
}
