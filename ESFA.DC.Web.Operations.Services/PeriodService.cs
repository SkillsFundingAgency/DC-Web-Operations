using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.CollectionsManagement.Models;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.PeriodEnd.Models;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Web.Operations.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.PeriodEnd;
using ESFA.DC.Web.Operations.Settings.Models;

namespace ESFA.DC.Web.Operations.Services
{
    public class PeriodService : BaseHttpClientService, IPeriodService
    {
        private const string NoPeriodError = "No return period found in PeriodService.";

        private readonly ILogger _logger;
        private readonly IDateTimeProvider _dateTimeProvider;

        private readonly string _baseUrl;

        public PeriodService(
            IRouteFactory routeFactory,
            ApiSettings apiSettings,
            IJsonSerializationService jsonSerializationService,
            HttpClient httpClient,
            ILogger logger,
            IDateTimeProvider dateTimeProvider)
            : base(routeFactory, jsonSerializationService, dateTimeProvider, httpClient)
        {
            _logger = logger;
            _dateTimeProvider = dateTimeProvider;
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

        public async Task<List<ReturnPeriod>> GetOpenPeriodsAsync(CancellationToken cancellationToken = default)
        {
            var periods = _jsonSerializationService.Deserialize<List<ReturnPeriod>>(
                await GetDataAsync($"{_baseUrl}/api/returns-calendar/open", cancellationToken));

            if (periods == null)
            {
                _logger.LogError(NoPeriodError);
                throw new Exception(NoPeriodError);
            }

            return periods;
        }

        public async Task<IDictionary<string, int>> GetAllPeriodsAsync(string ilrCollectionType, CancellationToken cancellationToken)
        {
            var maxIlrPeriods = _jsonSerializationService.Deserialize<int>(
                await GetDataAsync($"{_baseUrl}/api/returnperiod/maxPeriod/{ilrCollectionType}", cancellationToken));

            return Enumerable.Range(1, maxIlrPeriods).ToDictionary(x => $"R{x:00}");
        }

        public async Task<List<int>> GetValidityYearsAsync(
            string collectionType,
            string collectionName,
            CancellationToken cancellationToken)
        {
            var periods = _jsonSerializationService.Deserialize<List<ReturnPeriod>>(
                await GetDataAsync($"{_baseUrl}/api/returns-calendar/all/{collectionName}/{collectionType}", cancellationToken));

            if (periods == null)
            {
                _logger.LogError(NoPeriodError);
                throw new Exception(NoPeriodError);
            }

            return periods
                .Where(p => p.EndDateTimeUtc >= _dateTimeProvider.GetNowUtc())
                .OrderBy(p => p.CollectionYear)
                .Select(p => p.CollectionYear)
                .Distinct()
                .ToList();
        }
    }
}