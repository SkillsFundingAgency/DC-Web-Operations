using System;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.Processing;
using ESFA.DC.Web.Operations.Services.Hubs;

namespace ESFA.DC.Web.Operations.Services
{
    public sealed class JobFailedCurrentPeriodTimedHostedService : BaseTimedHostedService
    {
        private readonly IJobFailedCurrentPeriodService _jobFailedCurrentPeriodService;
        private readonly JobFailedCurrentPeriodHub _jobFailedCurrentPeriodHub;
        private readonly ILogger _logger;

        public JobFailedCurrentPeriodTimedHostedService(
            IJobFailedCurrentPeriodService jobFailedCurrentPeriodService,
            IJobFailedCurrentPeriodHubEventBase hubEventBase,
            JobFailedCurrentPeriodHub jobFailedCurrentPeriodHub,
            ISerialisationHelperService serialisationHelperService,
            ILogger logger)
            : base("Job Concern", logger, serialisationHelperService)
        {
            hubEventBase.ClientHeartbeatCallback += RegisterClient;
            _jobFailedCurrentPeriodService = jobFailedCurrentPeriodService;
            _jobFailedCurrentPeriodHub = jobFailedCurrentPeriodHub;
            _logger = logger;
        }

        protected override async Task DoWork(CancellationToken cancellationToken)
        {
            try
            {
                var jobs = await _jobFailedCurrentPeriodService.GetJobsFailedCurrentPeriodAsync(cancellationToken);
                await _jobFailedCurrentPeriodHub.SendMessage(jobs);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to {nameof(DoWork)} in {nameof(JobFailedCurrentPeriodTimedHostedService)}", ex);
            }
        }
    }
}
