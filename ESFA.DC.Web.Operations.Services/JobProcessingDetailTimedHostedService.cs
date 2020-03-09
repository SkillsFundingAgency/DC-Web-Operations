using System;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.Processing;
using ESFA.DC.Web.Operations.Services.Hubs;

namespace ESFA.DC.Web.Operations.Services
{
    public sealed class JobProcessingDetailTimedHostedService : BaseTimedHostedService
    {
        private readonly IJobProcessingDetailService _jobProcessingDetailService;
        private readonly JobProcessingDetailHub _jobProcessingDetailHub;
        private readonly ILogger _logger;

        public JobProcessingDetailTimedHostedService(
            IJobProcessingDetailService jobProcessingDetailService,
            IJobProcessingDetailHubEventBase hubEventBase,
            JobProcessingDetailHub jobProcessingDetailHub,
            ILogger logger)
            : base("Job Processing Detail", logger)
        {
            hubEventBase.ClientHeartbeatCallback += RegisterClient;
            _jobProcessingDetailService = jobProcessingDetailService;
            _jobProcessingDetailHub = jobProcessingDetailHub;
            _logger = logger;
        }

        protected override async Task DoWork(CancellationToken cancellationToken)
        {
            try
            {
                var jobs = await _jobProcessingDetailService.GetJobsThatAreProcessing(cancellationToken);
                await _jobProcessingDetailHub.SendMessage(jobs);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to {nameof(DoWork)} in {nameof(JobProcessingDetailTimedHostedService)}", ex);
            }
        }
    }
}
