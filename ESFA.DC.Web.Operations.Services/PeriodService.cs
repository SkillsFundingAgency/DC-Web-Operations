using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.CollectionsManagement.Models;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.PeriodEnd.Models;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.PeriodEnd;
using ESFA.DC.Web.Operations.Settings.Models;

namespace ESFA.DC.Web.Operations.Services
{
    public class PeriodService : BaseHttpClientService, IPeriodService
    {
        private const string NoPeriodError = "No return period found in PeriodService.";

        private readonly ILogger _logger;
        private readonly string _baseUrl;

        public PeriodService(
            ApiSettings apiSettings,
            IJsonSerializationService jsonSerializationService,
            HttpClient httpClient,
            ILogger logger)
            : base(jsonSerializationService, httpClient)
        {
            _logger = logger;
            _baseUrl = apiSettings.JobManagementApiBaseUrl;
        }

        public async Task<PathYearPeriod> ReturnPeriod(string collectionType, CancellationToken cancellationToken = default)
        {
            PathYearPeriod period = _jsonSerializationService.Deserialize<PathYearPeriod>(
                await GetDataAsync($"{_baseUrl}/api/returns-calendar/periodEnd/{collectionType}", cancellationToken));

            if (period == null)
            {
                _logger.LogError(NoPeriodError);
                throw new Exception(NoPeriodError);
            }

            return period;
        }

        public async Task<ReturnPeriod> GetRecentlyClosedPeriodAsync(CancellationToken cancellationToken = default)
        {
            ReturnPeriod period = _jsonSerializationService.Deserialize<ReturnPeriod>(
                await GetDataAsync($"{_baseUrl}/api/returns-calendar/closed", cancellationToken));

            if (period == null)
            {
                _logger.LogError(NoPeriodError);
                throw new Exception(NoPeriodError);
            }

            return period;
        }
    }
}