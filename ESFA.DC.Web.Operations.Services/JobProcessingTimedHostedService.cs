using System;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.Processing;
using ESFA.DC.Web.Operations.Services.Hubs;

namespace ESFA.DC.Web.Operations.Services
{
    public sealed class JobProcessingTimedHostedService : BaseTimedHostedService
    {
        private readonly IJobProcessingService _jobProcessingService;
        private readonly JobProcessingHub _jobProcessingHub;
        private readonly ILogger _logger;

        public JobProcessingTimedHostedService(
            IJobProcessingService jobProcessingService,
            IJobProcessingHubEventBase hubEventBase,
            JobProcessingHub jobProcessingHub,
            ILogger logger)
            : base("Job Processing", logger)
        {
            hubEventBase.ClientHeartbeatCallback += RegisterClient;
            _jobProcessingService = jobProcessingService;
            _jobProcessingHub = jobProcessingHub;
            _logger = logger;
        }

        protected override async Task DoWork(CancellationToken cancellationToken)
        {
            try
            {
                var jobs = await _jobProcessingService.GetJobsThatAreProcessing(cancellationToken);
                await _jobProcessingHub.SendMessage(jobs);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to {nameof(DoWork)} in {nameof(JobProcessingTimedHostedService)}", ex);
            }
        }
    }
}
