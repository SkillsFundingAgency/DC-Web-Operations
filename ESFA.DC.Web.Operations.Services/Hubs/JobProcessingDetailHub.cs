using System;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.Processing;
using Microsoft.AspNetCore.SignalR;

namespace ESFA.DC.Web.Operations.Services.Hubs
{
    public class JobProcessingDetailHub : BaseHub<JobProcessingDetailHub>
    {
        public JobProcessingDetailHub(IJobProcessingDetailHubEventBase eventBase, IHubContext<JobProcessingDetailHub> hubContext, ILogger logger)
            : base(eventBase, hubContext, logger)
        {
        }

        public async Task NewMessage(int duration)
        {
            await Clients.All.SendAsync("messageReceived", duration);
        }
    }
}
