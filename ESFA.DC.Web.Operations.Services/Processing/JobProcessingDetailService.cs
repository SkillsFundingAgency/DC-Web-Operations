using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.Jobs.Model.Processing.Detail;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Web.Operations.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.Processing;
using ESFA.DC.Web.Operations.Settings.Models;
using ESFA.DC.Web.Operations.Utils;

namespace ESFA.DC.Web.Operations.Services.Processing
{
    public class JobProcessingDetailService : BaseHttpClientService, IJobProcessingDetailService
    {
        private readonly string _baseUrl;

        public JobProcessingDetailService(
            IRouteFactory routeFactory,
            ApiSettings apiSettings,
            IJsonSerializationService jsonSerializationService,
            IDateTimeProvider dateTimeProvider,
            HttpClient httpClient)
            : base(routeFactory, jsonSerializationService, dateTimeProvider, httpClient)
        {
            _baseUrl = apiSettings.JobManagementApiBaseUrl;
        }

        public async Task<IEnumerable<JobDetails>> GetJobsProcessingDetails(short jobStatus, DateTime startDateTimeUtc, DateTime endDateTimeUtc, CancellationToken cancellationToken = default)
        {
            IEnumerable<JobDetails> jobDetailsList = new List<JobDetails>();
            var data = await GetDataAsync(
                $"{_baseUrl}/api/job/job-processing-details/{jobStatus}/{DateHelper.GetUrlFriendlyDate(startDateTimeUtc)}/{DateHelper.GetUrlFriendlyDate(endDateTimeUtc)}",
                cancellationToken);
            if (data != null)
            {
                jobDetailsList = _jsonSerializationService.Deserialize<IEnumerable<JobDetails>>(data);
            }

            return jobDetailsList;
        }

        public async Task<IEnumerable<JobDetails>> GetJobsProcessingDetailsForCurrentPeriod(short jobStatus, CancellationToken cancellationToken = default)
        {
            IEnumerable<JobDetails> jobDetailsList = new List<JobDetails>();
            var data = await GetDataAsync(
                $"{_baseUrl}/api/job/job-processing-details/current-period/{jobStatus}",
                cancellationToken);
            if (data != null)
            {
                jobDetailsList = _jsonSerializationService.Deserialize<IEnumerable<JobDetails>>(data);
            }

            return jobDetailsList;
        }
    }
}