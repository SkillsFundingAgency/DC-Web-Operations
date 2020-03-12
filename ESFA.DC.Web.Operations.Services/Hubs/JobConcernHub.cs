using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.Processing;
using Microsoft.AspNetCore.SignalR;

namespace ESFA.DC.Web.Operations.Services.Hubs
{
    public class JobConcernHub : BaseHub<JobConcernHub>
    {
        public JobConcernHub(IJobConcernHubEventBase eventBase, IHubContext<JobConcernHub> hubContext, ILogger logger)
            : base(eventBase, hubContext, logger)
        {
        }
    }
}