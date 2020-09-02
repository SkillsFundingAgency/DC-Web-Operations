using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.PeriodEnd.Models;
using ESFA.DC.Web.Operations.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.PeriodEnd;

namespace ESFA.DC.Web.Operations.Services.PeriodEnd
{
    public abstract class AbstractHistoryService : IHistoryService
    {
        private readonly string _baseUrl;
        private readonly IHttpClientService _httpClientService;

        protected AbstractHistoryService(
            string baseUrl,
            IHttpClientService httpClientService)
        {
            _baseUrl = baseUrl; // $"{apiSettings.JobManagementApiBaseUrl}/api/period-end-history";
            _httpClientService = httpClientService;
        }

        public async Task<IEnumerable<HistoryDetail>> GetHistoryDetails(int year, CancellationToken cancellationToken = default)
        {
            return await _httpClientService.GetAsync<IEnumerable<HistoryDetail>>(_baseUrl + $"/{year}", cancellationToken);
        }

        public async Task<IEnumerable<int>> GetCollectionYears(CancellationToken cancellationToken = default)
        {
            return await _httpClientService.GetAsync<IEnumerable<int>>(_baseUrl + "/years", cancellationToken);
        }
    }
}