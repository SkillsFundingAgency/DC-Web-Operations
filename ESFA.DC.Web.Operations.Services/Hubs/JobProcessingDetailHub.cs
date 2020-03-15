using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        private readonly ILogger _logger;

        public JobProcessingDetailHub(
            IJobProcessingDetailService jobProcessingDetailService,
            ILogger logger)
        {
            _jobProcessingDetailService = jobProcessingDetailService;
            _logger = logger;
        }

        public async Task<IEnumerable<JobDetails>> GetJobProcessingDetailsForLastHour()
        {
            return await _jobProcessingDetailService.GetJobsProcessingDetails((short)JobStatusType.Completed, DateTime.UtcNow.AddHours(-1), DateTime.UtcNow);
        }

        public async Task<IEnumerable<JobDetails>> GetJobProcessingDetailsForLastFiveMins()
        {
            return await _jobProcessingDetailService.GetJobsProcessingDetails((short)JobStatusType.Completed, DateTime.UtcNow.AddMinutes(-5), DateTime.UtcNow);
        }

        public async Task<IEnumerable<JobDetails>> GetJobProcessingDetailsForCurrentPeriod()
        {
            return await _jobProcessingDetailService.GetJobsProcessingDetailsForCurrentPeriod((short)JobStatusType.Completed);
        }
    }
}
