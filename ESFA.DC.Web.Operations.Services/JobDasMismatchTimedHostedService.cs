using System;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.PeriodEnd;
using ESFA.DC.Web.Operations.Interfaces.Processing;
using ESFA.DC.Web.Operations.Services.Hubs;

namespace ESFA.DC.Web.Operations.Services
{
    public sealed class JobDasMismatchTimedHostedService : BaseTimedHostedService
    {
        private readonly IJobDasMismatchService _jobDasMismatchService;
        private readonly JobDasMismatchHub _jobDasMismatchHub;
        private readonly ILogger _logger;

        public JobDasMismatchTimedHostedService(
            IJobDasMismatchService jobDasMismatchService,
            IJobDasMismatchHubEventBase hubEventBase,
            JobDasMismatchHub jobDasMismatchHub,
            ISerialisationHelperService serialisationHelperService,
            ILogger logger)
            : base("Job DasMismatch", logger, serialisationHelperService)
        {
            hubEventBase.ClientHeartbeatCallback += RegisterClient;
            _jobDasMismatchService = jobDasMismatchService;
            _jobDasMismatchHub = jobDasMismatchHub;
            _logger = logger;
        }

        protected override async Task DoWork(CancellationToken cancellationToken)
        {
            try
            {
                var jobs = await _jobDasMismatchService.GetDasMismatches(cancellationToken);
                await _jobDasMismatchHub.SendMessage(jobs);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to {nameof(DoWork)} in {nameof(JobDasMismatchTimedHostedService)}", ex);
            }
        }
    }
}
