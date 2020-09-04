using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Web.Operations.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.PeriodEnd;
using ESFA.DC.Web.Operations.Settings.Models;

namespace ESFA.DC.Web.Operations.Services.PeriodEnd
{
    public class ValidityPeriodService : BaseHttpClientService, IValidityPeriodService
    {
        private readonly string _baseUrl;

        public ValidityPeriodService(
            IRouteFactory routeFactory,
            IJsonSerializationService jsonSerializationService,
            ApiSettings apiSettings,
            IDateTimeProvider dateTimeProvider,
            HttpClient httpClient)
            : base(routeFactory, jsonSerializationService, dateTimeProvider, httpClient)
        {
            _baseUrl = apiSettings.JobManagementApiBaseUrl;
        }

        public async Task<string> GetValidityPeriodList(int collectionYear, int period, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await GetDataAsync($"{_baseUrl}/api/validityperiod/validityperiodlist/{collectionYear}/{period}", cancellationToken);
        }

        public async Task<string> UpdateValidityPeriod(int collectionYear, int period, object validityPeriods, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await PutDataAsync($"{_baseUrl}/api/validityperiod/updatevalidityperiod/{collectionYear}/{period}", validityPeriods, cancellationToken);
        }
    }
}