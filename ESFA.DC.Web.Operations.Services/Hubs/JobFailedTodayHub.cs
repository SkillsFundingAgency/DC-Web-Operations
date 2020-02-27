using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.Processing;
using Microsoft.AspNetCore.SignalR;

namespace ESFA.DC.Web.Operations.Services.Hubs
{
    public class JobFailedTodayHub : BaseHub<JobFailedTodayHub>
    {
        public JobFailedTodayHub(IJobFailedTodayHubEventBase eventBase, IHubContext<JobFailedTodayHub> hubContext, ILogger logger)
            : base(eventBase, hubContext, logger)
        {
        }
    }
}
