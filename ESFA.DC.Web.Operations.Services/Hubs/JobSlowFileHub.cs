using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.Processing;
using Microsoft.AspNetCore.SignalR;

namespace ESFA.DC.Web.Operations.Services.Hubs
{
    public class JobSlowFileHub : BaseHub<JobSlowFileHub>
    {
        public JobSlowFileHub(IJobSlowFileHubEventBase eventBase, IHubContext<JobSlowFileHub> hubContext, ILogger logger)
            : base(eventBase, hubContext, logger)
        {
        }
    }
}