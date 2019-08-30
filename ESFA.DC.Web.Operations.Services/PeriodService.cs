using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.CollectionsManagement.Models;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.PeriodEnd;
using ESFA.DC.Web.Operations.Models.PeriodEnd;
using ESFA.DC.Web.Operations.Settings.Models;

namespace ESFA.DC.Web.Operations.Services
{
    public class PeriodService : BaseHttpClientService, IPeriodService
    {
        private const string NoPeriodError = "No return period found in PeriodService.";

        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly ILogger _logger;
        private readonly string _baseUrl;

        public PeriodService(
            ApiSettings apiSettings,
            IJsonSerializationService jsonSerializationService,
            HttpClient httpClient,
            IDateTimeProvider dateTimeProvider,
            ILogger logger)
            : base(jsonSerializationService, httpClient)
        {
            _dateTimeProvider = dateTimeProvider;
            _logger = logger;
            _baseUrl = apiSettings.JobManagementApiBaseUrl;
        }

        public async Task<PathYearPeriod> ReturnPeriod(CancellationToken cancellationToken = default)
        {
            ReturnPeriod returnPeriod;
            var isClosed = false;

            var closedPeriod = _jsonSerializationService.Deserialize<ReturnPeriod>(
                await GetDataAsync($"{_baseUrl}/api/returns-calendar/closed", cancellationToken));
            var openPeriods = _jsonSerializationService.Deserialize<IEnumerable<ReturnPeriod>>(
                    await GetDataAsync($"{_baseUrl}/api/returns-calendar/open", cancellationToken)).ToList();

            if (closedPeriod != null && openPeriods.All(x => x.CollectionName != closedPeriod.CollectionName))
            {
                returnPeriod = closedPeriod;
                isClosed = true;
            }
            else
            {
                returnPeriod = openPeriods.OrderBy(op => op.EndDateTimeUtc).FirstOrDefault();
            }

            if (returnPeriod == null)
            {
                _logger.LogError(NoPeriodError);
                throw new Exception(NoPeriodError);
            }

            var collection = _jsonSerializationService.Deserialize<Collection>(
                await GetDataAsync($"{_baseUrl}/api/collections/name/{returnPeriod.CollectionName}", cancellationToken));

            var pathYearPeriod = new PathYearPeriod
            {
                Period = returnPeriod.PeriodNumber,
                Year = collection.CollectionYear,
                PeriodClosed = isClosed
            };

            return pathYearPeriod;
        }
    }
}