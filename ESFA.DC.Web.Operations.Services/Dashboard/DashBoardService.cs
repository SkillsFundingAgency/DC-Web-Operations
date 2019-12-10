using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Web.Operations.Interfaces.Dashboard;
using ESFA.DC.Web.Operations.Models.Dashboard;

namespace ESFA.DC.Web.Operations.Services.Dashboard
{
    public sealed class DashBoardService : IDashBoardService
    {
        private readonly IServiceBusStatsService _serviceBusStatsService;
        private readonly IJobService _jobService;

        public DashBoardService(
            IServiceBusStatsService serviceBusStatsService,
            IJobService jobService)
        {
            _serviceBusStatsService = serviceBusStatsService;
            _jobService = jobService;
        }

        public async Task<DashBoardModel> ProvideAsync(CancellationToken cancellationToken)
        {
            Task<ServiceBusStatusModel> serviceBusStats = _serviceBusStatsService.ProvideAsync(cancellationToken);
            Task<JobStatsModel> jobStats = _jobService.ProvideAsync(cancellationToken);

            await Task.WhenAll(serviceBusStats, jobStats);

            DashBoardModel dashBoardModel = new DashBoardModel
            {
                ServiceBusStats = serviceBusStats.Result,
                JobStats = jobStats.Result
            };

            return dashBoardModel;
        }
    }
}
