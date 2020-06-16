using System;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.Processing;
using ESFA.DC.Web.Operations.Services.Hubs;

namespace ESFA.DC.Web.Operations.Services
{
    public sealed class JobSubmittedTimedHostedService : BaseTimedHostedService
    {
        private readonly IJobSubmittedService _jobSubmittedService;
        private readonly JobSubmittedHub _jobSubmittedHub;
        private readonly ILogger _logger;

        public JobSubmittedTimedHostedService(
            IJobSubmittedService jobSubmittedService,
            IJobSubmittedHubEventBase hubEventBase,
            JobSubmittedHub jobSubmittedHub,
            ISerialisationHelperService serialisationHelperService,
            ILogger logger)
            : base("Job Submitted", logger, serialisationHelperService)
        {
            hubEventBase.ClientHeartbeatCallback += RegisterClient;
            _jobSubmittedService = jobSubmittedService;
            _jobSubmittedHub = jobSubmittedHub;
            _logger = logger;
        }

        protected override async Task DoWork(CancellationToken cancellationToken)
        {
            try
            {
                var jobs = await _jobSubmittedService.GetJobsThatAreSubmitted(cancellationToken);
                await _jobSubmittedHub.SendMessage(jobs);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to {nameof(DoWork)} in {nameof(JobSubmittedTimedHostedService)}", ex);
            }
        }
    }
}
