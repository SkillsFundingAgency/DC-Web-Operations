﻿using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Web.Operations.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.Processing;
using ESFA.DC.Web.Operations.Settings.Models;

namespace ESFA.DC.Web.Operations.Services.Processing
{
    public class JobFailedTodayService : BaseHttpClientService, IJobFailedTodayService
    {
        private readonly string _baseUrl;

        public JobFailedTodayService(
            IRouteFactory routeFactory,
            ApiSettings apiSettings,
            IJsonSerializationService jsonSerializationService,
            HttpClient httpClient)
            : base(routeFactory, jsonSerializationService, httpClient)
        {
            _baseUrl = apiSettings.JobManagementApiBaseUrl;
        }

        public async Task<string> GetJobsThatAreFailedToday(CancellationToken cancellationToken = default)
        {
            return await GetDataAsync($"{_baseUrl}/api/job/jobsthatarefailedtoday", cancellationToken);
        }
    }
}