﻿using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Web.Operations.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.Processing;
using ESFA.DC.Web.Operations.Settings.Models;

namespace ESFA.DC.Web.Operations.Services.Processing
{
    public class JobDasMismatchService : IJobDasMismatchService
    {
        private readonly IHttpClientService _httpClientService;
        private readonly string _baseUrl;

        public JobDasMismatchService(
            ApiSettings apiSettings,
            IHttpClientService httpClientService)
        {
            _httpClientService = httpClientService;
            _baseUrl = apiSettings.JobManagementApiBaseUrl;
        }

        public async Task<string> GetDasMismatches(CancellationToken cancellationToken)
        {
            return await _httpClientService.GetDataAsync($"{_baseUrl}/api/job/jobsthatareadasmismatch", cancellationToken);
        }
    }
}
