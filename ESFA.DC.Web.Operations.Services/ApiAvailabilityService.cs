using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Jobs.Model;
using ESFA.DC.Web.Operations.Interfaces;
using ESFA.DC.Web.Operations.Settings.Models;

namespace ESFA.DC.Web.Operations.Services
{
    public class ApiAvailabilityService : IApiAvailabilityService
    {
        private readonly IHttpClientService _httpClientService;
        private readonly string _baseUrl;

        public ApiAvailabilityService(
            ApiSettings apiSettings,
            IHttpClientService httpClientService)
        {
            _httpClientService = httpClientService;
            _baseUrl = apiSettings.JobManagementApiBaseUrl;
        }

        public async Task SetApiAvailabilityAsync(string apiName, string apiUpdateProcess, bool enabled, CancellationToken cancellationToken)
        {
            var apiAvailability = new ApiAvailabilityDto { ApiName = apiName, Process = apiUpdateProcess, Enabled = enabled };
            await _httpClientService.SendDataAsync($"{_baseUrl}/api/apiavailability/set", apiAvailability, cancellationToken);
        }
    }
}
