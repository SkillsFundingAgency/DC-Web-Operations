using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.Processing;
using Microsoft.AspNetCore.SignalR;

namespace ESFA.DC.Web.Operations.Services.Hubs
{
    public class ProvidersReturnedCurrentPeriodHub : BaseHub<ProvidersReturnedCurrentPeriodHub>
    {
        public ProvidersReturnedCurrentPeriodHub(IJobProvidersReturnedCurrentPeriodHubEventBase eventBase, IHubContext<ProvidersReturnedCurrentPeriodHub> hubContext, ILogger logger)
            : base(eventBase, hubContext, logger)
        {
        }
    }
}