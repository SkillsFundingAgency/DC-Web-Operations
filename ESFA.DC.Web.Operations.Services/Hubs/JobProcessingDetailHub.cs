using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.Jobs.Model.Enums;
using ESFA.DC.Jobs.Model.Processing.Detail;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.Processing;
using Microsoft.AspNetCore.SignalR;

namespace ESFA.DC.Web.Operations.Services.Hubs
{
    public class JobProcessingDetailHub : Hub
    {
        private readonly IJobProcessingDetailService _jobProcessingDetailService;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly ILogger _logger;
        private const int LastHour = -60;
        private const int LastFiveMins = -5;

        public JobProcessingDetailHub(
            IJobProcessingDetailService jobProcessingDetailService,
            IDateTimeProvider dateTimeProvider,
            ILogger logger)
        {
            _jobProcessingDetailService = jobProcessingDetailService;
            _dateTimeProvider = dateTimeProvider;
            _logger = logger;
        }

        public async Task<IEnumerable<JobDetails>> GetJobProcessingDetailsForLastHour()
        {
            return await GetJobsProcessingDetails(LastHour);
        }

        public async Task<IEnumerable<JobDetails>> GetJobProcessingDetailsForLastFiveMins()
        {
            return await GetJobsProcessingDetails(LastFiveMins);
        }

        public async Task<IEnumerable<JobDetails>> GetJobProcessingDetailsForCurrentPeriod()
        {
            return await _jobProcessingDetailService.GetJobsProcessingDetailsForCurrentPeriod((short)JobStatusType.Completed);
        }

        private async Task<IEnumerable<JobDetails>> GetJobsProcessingDetails(int minutes)
        {
            return await _jobProcessingDetailService.GetJobsProcessingDetails(
                (short) JobStatusType.Completed,
                _dateTimeProvider.GetNowUtc().AddMinutes(minutes),
                _dateTimeProvider.GetNowUtc());
        }
    }
}
