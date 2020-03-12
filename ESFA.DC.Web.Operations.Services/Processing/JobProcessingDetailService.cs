using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Jobs.Model.Processing.Detail;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.Processing;
using ESFA.DC.Web.Operations.Settings.Models;
using ESFA.DC.Web.Operations.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ESFA.DC.Web.Operations.Services.Processing
{
    public class JobProcessingDetailService : BaseHttpClientService, IJobProcessingDetailService
    {
        private readonly string _baseUrl;

        public JobProcessingDetailService(ApiSettings apiSettings, IJsonSerializationService jsonSerializationService, HttpClient httpClient)
            : base(jsonSerializationService, httpClient)
        {
            _baseUrl = apiSettings.JobManagementApiBaseUrl;
        }

        public async Task<IEnumerable<JobDetails>> GetJobsProcessingDetails(DateTime startDateTimeUtc, DateTime endDateTimeUtc, CancellationToken cancellationToken = default)
        {
            IEnumerable<JobDetails> jobDetailsList = new List<JobDetails>();
            var data = await GetDataAsync(
                $"{_baseUrl}/api/job/job-processing-details/4/{DateHelper.GetUrlFriendlyDate(startDateTimeUtc)}/{DateHelper.GetUrlFriendlyDate(endDateTimeUtc)}",
                cancellationToken);
            if (data != null)
            {
                jobDetailsList = _jsonSerializationService.Deserialize<IEnumerable<JobDetails>>(data);
            }

            return jobDetailsList;
        }
    }
}
