using System;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.Processing;
using ESFA.DC.Web.Operations.Services.Hubs;

namespace ESFA.DC.Web.Operations.Services
{
    public sealed class JobConcernTimedHostedService : BaseTimedHostedService
    {
        private readonly IJobConcernService _jobConcernService;
        private readonly JobConcernHub _jobConcernHub;
        private readonly ILogger _logger;

        public JobConcernTimedHostedService(
            IJobConcernService jobConcernService,
            IJobConcernHubEventBase hubEventBase,
            JobConcernHub jobConcernHub,
            ILogger logger)
            : base("Job Concern", logger)
        {
            hubEventBase.ClientHeartbeatCallback += RegisterClient;
            _jobConcernService = jobConcernService;
            _jobConcernHub = jobConcernHub;
            _logger = logger;
        }

        protected override async Task DoWork(CancellationToken cancellationToken)
        {
            try
            {
                var jobs = await _jobConcernService.GetJobsThatAreConcern(cancellationToken);
                await _jobConcernHub.SendMessage(jobs);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to {nameof(DoWork)} in {nameof(JobConcernTimedHostedService)}", ex);
            }
        }
    }
}
