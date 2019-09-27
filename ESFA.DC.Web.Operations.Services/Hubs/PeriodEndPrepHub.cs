using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Web.Operations.Interfaces.PeriodEnd;
using Microsoft.AspNetCore.SignalR;

namespace ESFA.DC.Web.Operations.Services.Hubs
{
    public class PeriodEndPrepHub : Hub
    {
        private readonly IHubEventBase _eventBase;
        private readonly IHubContext<PeriodEndPrepHub> _hubContext;
        private readonly IPeriodEndService _periodEndService;

        public PeriodEndPrepHub(
            IHubEventBase eventBase,
            IHubContext<PeriodEndPrepHub> hubContext,
            IPeriodEndService periodEndService)
        {
            _eventBase = eventBase;
            _hubContext = hubContext;
            _periodEndService = periodEndService;
        }

        public async Task SendMessage(string referenceJobs, string failedJobs, CancellationToken cancellationToken)
        {
            await _hubContext.Clients.All.SendAsync("ReceiveMessage", referenceJobs, failedJobs, cancellationToken);
        }

        public async Task ReceiveMessage()
        {
            _eventBase.TriggerPeriodEndPrep();
        }

        public async Task ReSubmitJob(int jobId)
        {
            await _hubContext.Clients.All.SendAsync("DisableJobReSubmit", jobId);
            await _periodEndService.ReSubmitFailedJob(jobId);
        }
    }
}