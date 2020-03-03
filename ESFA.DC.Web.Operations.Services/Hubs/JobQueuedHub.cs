using System;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.Processing;
using Microsoft.AspNetCore.SignalR;

namespace ESFA.DC.Web.Operations.Services.Hubs
{
    public class JobQueuedHub : BaseHub<JobQueuedHub>
    {
        public JobQueuedHub(IJobQueuedHubEventBase eventBase, IHubContext<JobQueuedHub> hubContext, ILogger logger)
            : base(eventBase, hubContext, logger)
        {
        }
    }
}
