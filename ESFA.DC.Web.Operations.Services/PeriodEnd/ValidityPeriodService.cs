using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.PeriodEnd;
using ESFA.DC.Web.Operations.Services.Hubs;
using ESFA.DC.Web.Operations.Settings.Models;

namespace ESFA.DC.Web.Operations.Services.PeriodEnd
{
    public class ValidityPeriodService : BaseHttpClientService, IValidityPeriodService
    {
        private readonly ILogger _logger;
        private readonly string _baseUrl;

        public ValidityPeriodService(
            IJsonSerializationService jsonSerializationService,
            ILogger logger,
            ApiSettings apiSettings,
            HttpClient httpClient)
            : base(jsonSerializationService, httpClient)
        {
            _logger = logger;
            _baseUrl = apiSettings.JobManagementApiBaseUrl;
        }

        public async Task<string> GetValidityPeriodList(int period, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await GetDataAsync($"{_baseUrl}/api/validityperiod/validityperiodlist/{period}", cancellationToken);
        }

        public async Task<string> UpdateValidityPeriod(int period, object validityPeriods, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await PutDataAsync($"{_baseUrl}/api/validityperiod/updatevalidityperiod/{period}", validityPeriods, cancellationToken);
        }
    }
}