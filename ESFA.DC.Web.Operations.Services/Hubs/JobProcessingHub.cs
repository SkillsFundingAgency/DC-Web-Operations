using System;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.Processing;
using Microsoft.AspNetCore.SignalR;

namespace ESFA.DC.Web.Operations.Services.Hubs
{
    public class JobProcessingHub : BaseHub<JobProcessingHub>
    {
        public JobProcessingHub(IJobProcessingHubEventBase eventBase, IHubContext<JobProcessingHub> hubContext, ILogger logger)
            : base(eventBase, hubContext, logger)
        {
        }
    }
}
