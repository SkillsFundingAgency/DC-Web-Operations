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
            await GetDataAsync($"{_baseUrl}/api/periodend/startPeriodEnd/{year}/{period}", cancellationToken);
        }

        public async Task Proceed(int year, int period, int path = 0, CancellationToken cancellationToken = default(CancellationToken))
        {
            await GetDataAsync(_baseUrl + $"/api/periodend/proceed/{year}/{period}/{path}", cancellationToken);
        }

        public async Task ToggleReferenceDataJobs(bool pause, CancellationToken cancellationToken = default(CancellationToken))
        {
            await GetDataAsync(_baseUrl + $"/api/periodend/referenceDataJobs/{pause}", cancellationToken);
        }

        public async Task PublishReports(int year, int period, CancellationToken cancellationToken = default(CancellationToken))
        {
            await GetDataAsync(_baseUrl + $"/api/periodend/publishReports/{year}/{period}", cancellationToken);
        }

        public async Task<string> GetPathItemStates(int year, int period, CancellationToken cancellationToken = default(CancellationToken))
        {
            string data = await GetDataAsync(_baseUrl + $"/api/periodend/getStates/{year}/{period}", cancellationToken);
            return data;
        }
    }
}