﻿using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.Processing;
using ESFA.DC.Web.Operations.Settings.Models;

namespace ESFA.DC.Web.Operations.Services.Processing
{
    public class JobSubmittedService : BaseHttpClientService, IJobSubmittedService
    {
        private readonly string _baseUrl;

        public JobSubmittedService(ApiSettings apiSettings, IJsonSerializationService jsonSerializationService, HttpClient httpClient)
            : base(jsonSerializationService, httpClient)
        {
            _baseUrl = apiSettings.JobManagementApiBaseUrl;
        }

        public async Task<string> GetJobsThatAreSubmitted(CancellationToken cancellationToken = default)
        {
            return await GetDataAsync($"{_baseUrl}/api/job/jobsthataresubmitted", cancellationToken);
        }
    }
}