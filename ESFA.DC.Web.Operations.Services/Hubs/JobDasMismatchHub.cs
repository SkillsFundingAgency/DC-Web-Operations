using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.Processing;
using Microsoft.AspNetCore.SignalR;

namespace ESFA.DC.Web.Operations.Services.Hubs
{
    public class JobDasMismatchHub : BaseHub<JobDasMismatchHub>
    {
        public JobDasMismatchHub(IJobDasMismatchHubEventBase eventBase, IHubContext<JobDasMismatchHub> hubContext, ILogger logger)
            : base(eventBase, hubContext, logger)
        {
        }
    }
}