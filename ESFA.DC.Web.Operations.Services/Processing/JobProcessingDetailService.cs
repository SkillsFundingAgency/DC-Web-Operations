using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Jobs.Model.Processing.Detail;
using ESFA.DC.Web.Operations.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.Processing;
using ESFA.DC.Web.Operations.Settings.Models;
using ESFA.DC.Web.Operations.Utils;

namespace ESFA.DC.Web.Operations.Services.Processing
{
    public class JobProcessingDetailService : IJobProcessingDetailService
    {
        private readonly IHttpClientService _httpClientService;
        private readonly string _baseUrl;

        public JobProcessingDetailService(
            ApiSettings apiSettings,
            IHttpClientService httpClientService)
        {
            _httpClientService = httpClientService;
            _baseUrl = apiSettings.JobManagementApiBaseUrl;
        }

        public async Task<IEnumerable<JobDetails>> GetJobsProcessingDetails(short jobStatus, DateTime startDateTimeUtc, DateTime endDateTimeUtc, CancellationToken cancellationToken = default)
        {
            var url = $"{_baseUrl}/api/job/job-processing-details/{jobStatus}/{DateHelper.GetUrlFriendlyDate(startDateTimeUtc)}/{DateHelper.GetUrlFriendlyDate(endDateTimeUtc)}";

            var data = await _httpClientService.GetAsync<IEnumerable<JobDetails>>(url, cancellationToken);

            return data ?? Enumerable.Empty<JobDetails>();
        }

        public async Task<IEnumerable<JobDetails>> GetJobsProcessingDetailsForCurrentPeriod(short jobStatus, CancellationToken cancellationToken = default)
        {
            var url = $"{_baseUrl}/api/job/job-processing-details/current-period/{jobStatus}";

            var data = await _httpClientService.GetAsync<IEnumerable<JobDetails>>(url, cancellationToken);

            return data ?? Enumerable.Empty<JobDetails>();
        }
    }
}