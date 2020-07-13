using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.PeriodEnd.Models;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Web.Operations.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.PeriodEnd;

namespace ESFA.DC.Web.Operations.Services.PeriodEnd
{
    public abstract class AbstractHistoryService : BaseHttpClientService, IHistoryService
    {
        private readonly string _baseUrl;

        protected AbstractHistoryService(
            IRouteFactory routeFactory,
            IJsonSerializationService jsonSerializationService,
            HttpClient httpClient,
            string baseUrl)
            : base(routeFactory, jsonSerializationService, httpClient)
        {
            _baseUrl = baseUrl; // $"{apiSettings.JobManagementApiBaseUrl}/api/period-end-history";
        }

        public async Task<IEnumerable<HistoryDetail>> GetHistoryDetails(int year, CancellationToken cancellationToken = default)
        {
            var data = await GetDataAsync(_baseUrl + $"/{year}", cancellationToken);

            var result = _jsonSerializationService.Deserialize<IEnumerable<HistoryDetail>>(data);
            return result;
        }

        public async Task<IEnumerable<int>> GetCollectionYears(CancellationToken cancellationToken = default)
        {
            var data = await GetDataAsync(_baseUrl + "/years", cancellationToken);

            var result = _jsonSerializationService.Deserialize<IEnumerable<int>>(data);
            return result;
        }
    }
}