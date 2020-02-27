using System;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.Processing;
using ESFA.DC.Web.Operations.Services.Hubs;

namespace ESFA.DC.Web.Operations.Services
{
    public sealed class JobSlowFileTimedHostedService : BaseTimedHostedService
    {
        private readonly IJobSlowFileService _jobSlowFileService;
        private readonly JobSlowFileHub _jobSlowFileHub;
        private readonly ILogger _logger;

        public JobSlowFileTimedHostedService(
            IJobSlowFileService jobSlowFileService,
            IJobSlowFileHubEventBase hubEventBase,
            JobSlowFileHub jobSlowFileHub,
            ILogger logger)
            : base("Job Slow File", logger)
        {
            hubEventBase.ClientHeartbeatCallback += RegisterClient;
            _jobSlowFileService = jobSlowFileService;
            _jobSlowFileHub = jobSlowFileHub;
            _logger = logger;
        }

        protected override async Task DoWork(CancellationToken cancellationToken)
        {
            try
            {
                var jobs = await _jobSlowFileService.GetJobsThatAreSlowFile(cancellationToken);
                await _jobSlowFileHub.SendMessage(jobs);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to {nameof(DoWork)} in {nameof(JobSlowFileTimedHostedService)}", ex);
            }
        }
    }
}
