using System;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.PeriodEnd;
using ESFA.DC.Web.Operations.Interfaces.Processing;
using ESFA.DC.Web.Operations.Services.Hubs;

namespace ESFA.DC.Web.Operations.Services
{
    public sealed class JobQueuedTimedHostedService : BaseTimedHostedService
    {
        private readonly IJobQueuedService _jobQueuedService;
        private readonly JobQueuedHub _jobQueuedHub;
        private readonly ILogger _logger;

        public JobQueuedTimedHostedService(
            IJobQueuedService jobQueuedService,
            IJobQueuedHubEventBase hubEventBase,
            JobQueuedHub jobQueuedHub,
            ISerialisationHelperService serialisationHelperService,
            ILogger logger)
            : base("Job Queued", logger, serialisationHelperService)
        {
            hubEventBase.ClientHeartbeatCallback += RegisterClient;
            _jobQueuedService = jobQueuedService;
            _jobQueuedHub = jobQueuedHub;
            _logger = logger;
        }

        protected override async Task DoWork(CancellationToken cancellationToken)
        {
            try
            {
                var jobs = await _jobQueuedService.GetJobsThatAreQueued(cancellationToken);
                await _jobQueuedHub.SendMessage(jobs);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to {nameof(DoWork)} in {nameof(JobQueuedTimedHostedService)}", ex);
            }
        }
    }
}
