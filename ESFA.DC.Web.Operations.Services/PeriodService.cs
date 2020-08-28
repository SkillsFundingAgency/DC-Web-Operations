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
    public class PeriodService : IPeriodService
    {
        private const string NoPeriodError = "No return period found in PeriodService.";

        private readonly ILogger _logger;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IHttpClientService _httpClientService;

        private readonly string _baseUrl;

        public PeriodService(
            ApiSettings apiSettings,
            ILogger logger,
            IDateTimeProvider dateTimeProvider,
            IHttpClientService httpClientService)
        {
            _logger = logger;
            _dateTimeProvider = dateTimeProvider;
            _httpClientService = httpClientService;
            _baseUrl = apiSettings.JobManagementApiBaseUrl;
        }

        public async Task<PathYearPeriod> ReturnPeriod(string collectionType, CancellationToken cancellationToken = default)
        {
            PathYearPeriod period = await _httpClientService.GetAsync<PathYearPeriod>($"{_baseUrl}/api/returns-calendar/periodEnd/{collectionType}", cancellationToken);

            AssertPeriodResponseValid(period);

            return period;
        }

        public async Task<ReturnPeriod> GetRecentlyClosedPeriodAsync(CancellationToken cancellationToken = default)
        {
            ReturnPeriod period = await _httpClientService.GetAsync<ReturnPeriod>($"{_baseUrl}/api/returns-calendar/closed", cancellationToken);

            AssertPeriodResponseValid(period);

            return period;
        }

        public async Task<List<ReturnPeriod>> GetOpenPeriodsAsync(CancellationToken cancellationToken = default)
        {
            var periods = await _httpClientService.GetAsync<List<ReturnPeriod>>($"{_baseUrl}/api/returns-calendar/open", cancellationToken);

            AssertPeriodResponseValid(periods);

            return periods;
        }

        public async Task<IDictionary<string, int>> GetAllPeriodsAsync(string ilrCollectionType, CancellationToken cancellationToken)
        {
            var maxIlrPeriods = await _httpClientService.GetAsync<int>($"{_baseUrl}/api/returnperiod/maxPeriod/{ilrCollectionType}", cancellationToken);

            return Enumerable.Range(1, maxIlrPeriods).ToDictionary(x => $"R{x:00}");
        }

        public async Task<List<int>> GetValidityYearsAsync(
            string collectionType,
            string collectionName,
            CancellationToken cancellationToken)
        {
            var periods = await _httpClientService.GetAsync<List<ReturnPeriod>>($"{_baseUrl}/api/returns-calendar/all/{collectionName}/{collectionType}", cancellationToken);

            AssertPeriodResponseValid(periods);

            return periods
                .Where(p => p.EndDateTimeUtc >= _dateTimeProvider.GetNowUtc())
                .OrderBy(p => p.CollectionYear)
                .Select(p => p.CollectionYear)
                .Distinct()
                .ToList();
        }

        private void AssertPeriodResponseValid<T>(T response)
            where T : class
        {
            if (response == null)
            {
                _logger.LogError(NoPeriodError);
                throw new Exception(NoPeriodError);
            }
        }
    }
}