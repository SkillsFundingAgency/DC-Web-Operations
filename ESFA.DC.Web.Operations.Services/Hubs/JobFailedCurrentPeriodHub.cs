using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.Processing;
using Microsoft.AspNetCore.SignalR;

namespace ESFA.DC.Web.Operations.Services.Hubs
{
    public class JobFailedCurrentPeriodHub : BaseHub<JobFailedCurrentPeriodHub>
    {
        public JobFailedCurrentPeriodHub(IJobFailedCurrentPeriodHubEventBase eventBase, IHubContext<JobFailedCurrentPeriodHub> hubContext, ILogger logger)
            : base(eventBase, hubContext, logger)
        {
        }
    }
}