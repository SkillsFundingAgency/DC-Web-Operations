using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.PeriodEnd;
using ESFA.DC.Web.Operations.Settings.Models;

namespace ESFA.DC.Web.Operations.Services.PeriodEnd
{
    public class PeriodEndService : BaseHttpClientService, IPeriodEndService
    {
        private readonly string _baseUrl;

        public PeriodEndService(
            IJsonSerializationService jsonSerializationService,
            ApiSettings apiSettings,
            HttpClient httpClient)
            : base(jsonSerializationService, httpClient)
        {
            _baseUrl = apiSettings.JobManagementApiBaseUrl;
        }

        public async Task StartPeriodEnd(int year, int period, CancellationToken cancellationToken = default(CancellationToken))
        {
            await GetDataAsync($"{_baseUrl}/api/periodend/proceed/{year}/{period}", cancellationToken);
        }

        public async Task Proceed(int startIndex = 0, CancellationToken cancellationToken = default(CancellationToken))
        {
            await GetDataAsync(_baseUrl + "/api/periodend/proceed/" + startIndex, cancellationToken);
        }

        public async Task<string> GetPathItemStates(CancellationToken cancellationToken = default(CancellationToken))
        {
            string data = await GetDataAsync(_baseUrl + "/api/periodend/getStates/", cancellationToken);
            return data;
        }
    }
}