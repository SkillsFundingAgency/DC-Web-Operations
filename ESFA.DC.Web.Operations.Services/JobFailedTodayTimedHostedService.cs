using System;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.Processing;
using ESFA.DC.Web.Operations.Services.Hubs;

namespace ESFA.DC.Web.Operations.Services
{
    public sealed class JobFailedTodayTimedHostedService : BaseTimedHostedService
    {
        private readonly IJobFailedTodayService _jobFailedTodayService;
        private readonly JobFailedTodayHub _jobFailedTodayHub;
        private readonly ILogger _logger;

        public JobFailedTodayTimedHostedService(
            IJobFailedTodayService jobFailedTodayService,
            IJobFailedTodayHubEventBase hubEventBase,
            JobFailedTodayHub jobFailedTodayHub,
            ILogger logger)
            : base("Job FailedToday", logger)
        {
            hubEventBase.ClientHeartbeatCallback += RegisterClient;
            _jobFailedTodayService = jobFailedTodayService;
            _jobFailedTodayHub = jobFailedTodayHub;
            _logger = logger;
        }

        protected override async Task DoWork(CancellationToken cancellationToken)
        {
            try
            {
                var jobs = await _jobFailedTodayService.GetJobsThatAreFailedToday(cancellationToken);
                await _jobFailedTodayHub.SendMessage(jobs);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to {nameof(DoWork)} in {nameof(JobFailedTodayTimedHostedService)}", ex);
            }
        }
    }
}
