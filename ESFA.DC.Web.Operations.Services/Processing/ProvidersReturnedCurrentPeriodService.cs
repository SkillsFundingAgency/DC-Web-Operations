using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Web.Operations.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.Processing;
using ESFA.DC.Web.Operations.Settings.Models;

namespace ESFA.DC.Web.Operations.Services.Processing
{
    public class ProvidersReturnedCurrentPeriodService : BaseHttpClientService, IJobProvidersReturnedCurrentPeriodService
    {
        private readonly string _baseUrl;

        public ProvidersReturnedCurrentPeriodService(
            IRouteFactory routeFactory,
            ApiSettings apiSettings,
            IJsonSerializationService jsonSerializationService,
            IDateTimeProvider dateTimeProvider,
            HttpClient httpClient)
            : base(routeFactory, jsonSerializationService, dateTimeProvider, httpClient)
        {
            _baseUrl = apiSettings.JobManagementApiBaseUrl;
        }

        public async Task<string> GetProvidersReturnedCurrentPeriodAsync(CancellationToken cancellationToken)
        {
            return await GetDataAsync($"{_baseUrl}/api/job/providersReturnedCurrentPeriod", cancellationToken);
        }
    }
}
