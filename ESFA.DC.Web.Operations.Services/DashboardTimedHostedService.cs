using System;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.Dashboard;
using ESFA.DC.Web.Operations.Services.Hubs;

namespace ESFA.DC.Web.Operations.Services
{
    public sealed class DashboardTimedHostedService : BaseTimedHostedService
    {
        private readonly IDashBoardService _dashBoardService;
        private readonly DashBoardHub _dashboardHub;
        private readonly ILogger _logger;

        public DashboardTimedHostedService(
            IDashBoardService dashBoardService,
            IDashBoardHubEventBase hubEventBase,
            DashBoardHub dashboardHub,
            ILogger logger)
            : base("Dashboard", logger)
        {
            hubEventBase.ClientHeartbeatCallback += RegisterClient;
            _dashBoardService = dashBoardService;
            _dashboardHub = dashboardHub;
            _logger = logger;
        }

        protected override async Task DoWork(CancellationToken cancellationToken)
        {
            try
            {
                await _dashboardHub.SendMessage(await _dashBoardService.ProvideAsync(cancellationToken));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to {nameof(DoWork)} in {nameof(PeriodEndTimedHostedService)}", ex);
            }
        }
    }
}
