using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Web.Operations.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.PeriodEnd;
using ESFA.DC.Web.Operations.Settings.Models;

namespace ESFA.DC.Web.Operations.Services.PeriodEnd
{
    public class ValidityPeriodService : IValidityPeriodService
    {
        private readonly IHttpClientService _httpClientService;
        private readonly string _baseUrl;

        public ValidityPeriodService(
            ApiSettings apiSettings,
            IHttpClientService httpClientService)
        {
            _httpClientService = httpClientService;
            _baseUrl = apiSettings.JobManagementApiBaseUrl;
        }

        public async Task<string> GetValidityPeriodList(int collectionYear, int period, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await _httpClientService.GetDataAsync($"{_baseUrl}/api/validityperiod/validityperiodlist/{collectionYear}/{period}", cancellationToken);
        }

        public async Task<string> UpdateValidityPeriod(int collectionYear, int period, object validityPeriods, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await _httpClientService.PutDataAsync($"{_baseUrl}/api/validityperiod/updatevalidityperiod/{collectionYear}/{period}", validityPeriods, cancellationToken);
        }

        public async Task<string> GetValidityStructure(int collectionYear, int period, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await _httpClientService.GetDataAsync($"{_baseUrl}/api/validityperiod/allvaliditiesperperiod/{collectionYear}/{period}", cancellationToken);
        }
    }
}