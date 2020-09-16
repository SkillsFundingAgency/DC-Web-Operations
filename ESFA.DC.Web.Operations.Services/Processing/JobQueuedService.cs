using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Web.Operations.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.Processing;
using ESFA.DC.Web.Operations.Settings.Models;

namespace ESFA.DC.Web.Operations.Services.Processing
{
    public class JobQueuedService : IJobQueuedService
    {
        private readonly IHttpClientService _httpClientService;
        private readonly string _baseUrl;

        public JobQueuedService(
            ApiSettings apiSettings,
            IHttpClientService httpClientService)
        {
            _httpClientService = httpClientService;
            _baseUrl = apiSettings.JobManagementApiBaseUrl;
        }

        public async Task<string> GetJobsThatAreQueued(CancellationToken cancellationToken = default)
        {
            return await _httpClientService.GetDataAsync($"{_baseUrl}/api/job/jobsthatarequeued", cancellationToken);
        }
    }
}
