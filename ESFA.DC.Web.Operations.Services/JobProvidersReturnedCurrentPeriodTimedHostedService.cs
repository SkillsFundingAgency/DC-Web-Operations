using System;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.Processing;
using ESFA.DC.Web.Operations.Services.Hubs;

namespace ESFA.DC.Web.Operations.Services
{
    public sealed class JobProvidersReturnedCurrentPeriodTimedHostedService : BaseTimedHostedService
    {
        private readonly IJobProvidersReturnedCurrentPeriodService _providersReturnedCurrentPeriodService;
        private readonly ProvidersReturnedCurrentPeriodHub _providersReturnedCurrentPeriodHub;
        private readonly ILogger _logger;

        public JobProvidersReturnedCurrentPeriodTimedHostedService(
            IJobProvidersReturnedCurrentPeriodService providersReturnedCurrentPeriodService,
            IJobProvidersReturnedCurrentPeriodHubEventBase hubEventBase,
            ProvidersReturnedCurrentPeriodHub providersReturnedCurrentPeriodHub,
            ISerialisationHelperService serialisationHelperService,
            ILogger logger)
            : base("Providers Returned Current Period", logger, serialisationHelperService)
        {
            hubEventBase.ClientHeartbeatCallback += RegisterClient;
            _providersReturnedCurrentPeriodHub = providersReturnedCurrentPeriodHub;
            _providersReturnedCurrentPeriodService = providersReturnedCurrentPeriodService;
            _logger = logger;
        }

        protected override async Task DoWork(CancellationToken cancellationToken)
        {
            try
            {
                var jobs = await _providersReturnedCurrentPeriodService.GetProvidersReturnedCurrentPeriodAsync(cancellationToken);
                await _providersReturnedCurrentPeriodHub.SendMessage(jobs);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to {nameof(DoWork)} in {nameof(JobProvidersReturnedCurrentPeriodTimedHostedService)}", ex);
            }
        }
    }
}
