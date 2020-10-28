using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Web.Operations.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.Processing;
using ESFA.DC.Web.Operations.Settings.Models;

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

        public async Task<string> GetJobsProcessingDetailsForCurrentPeriod(short jobStatus, CancellationToken cancellationToken)
        {
            return await _httpClientService.GetDataAsync($"{_baseUrl}/api/job/jobs-processing-details/current-period/{jobStatus}", cancellationToken);
        }

        public async Task<string> GetJobsProcessingDetailsForCurrentPeriodLast5Mins(short jobStatus, CancellationToken cancellationToken)
        {
            return await _httpClientService.GetDataAsync($"{_baseUrl}/api/job/jobs-processing-details/current-period/last5mins/{jobStatus}", cancellationToken);
        }

        public async Task<string> GetJobsProcessingDetailsForCurrentPeriodLastHour(short jobStatus, CancellationToken cancellationToken)
        {
            return await _httpClientService.GetDataAsync($"{_baseUrl}/api/job/jobs-processing-details/current-period/lasthour/{jobStatus}", cancellationToken);
        }
    }
}