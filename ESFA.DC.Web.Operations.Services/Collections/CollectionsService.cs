using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.Collections;
using ESFA.DC.Web.Operations.Models.Collection;
using ESFA.DC.Web.Operations.Settings.Models;
using ReturnPeriod = ESFA.DC.Web.Operations.Models.Collection.ReturnPeriod;

namespace ESFA.DC.Web.Operations.Services.Collections
{
    public class CollectionsService : BaseHttpClientService, ICollectionsService
    {
        private readonly string _baseUrl;
        private readonly string[] _collectionsTypesToExclude = { "REF", "PE", "FRM" };
        private readonly IDateTimeProvider _dateTimeProvider;

        public CollectionsService(
            IJsonSerializationService jsonSerializationService,
            ApiSettings apiSettings,
            HttpClient httpClient,
            IDateTimeProvider dateTimeProvider)
            : base(jsonSerializationService, httpClient)
        {
            _baseUrl = apiSettings.JobManagementApiBaseUrl;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<IEnumerable<CollectionSummary>> GetAllCollectionSummariesForYear(int year, CancellationToken cancellationToken = default(CancellationToken))
        {
            var data = _jsonSerializationService.Deserialize<IEnumerable<CollectionsManagement.Models.Collection>>(
                await GetDataAsync($"{_baseUrl}/api/collections/for-year/{year}", cancellationToken));

            return data.Where(c => !_collectionsTypesToExclude.Contains(c.CollectionType))
                .Select(collection => new CollectionSummary()
                {
                    CollectionId = collection.CollectionId,
                    CollectionYear = year,
                    CollectionName = collection.CollectionTitle,
                    IsCollectionEnabled = collection.IsOpen,
                    CollectionClosedDate = collection.ClosingDate,
                    CollectionCurrentOrLastPeriod = collection.CurrentOrLastPeriod
                })
                .ToList();
        }

        public async Task<IEnumerable<int>> GetAvailableCollectionYears(CancellationToken cancellationToken = default(CancellationToken))
        {
            return _jsonSerializationService.Deserialize<IEnumerable<int>>(
                await GetDataAsync($"{_baseUrl}/api/collections/available-years", cancellationToken));
        }

        public async Task<CollectionsManagement.Models.Collection> GetCollectionById(int collectionId, CancellationToken cancellationToken = default(CancellationToken))
        {
          return _jsonSerializationService.Deserialize<CollectionsManagement.Models.Collection>(
                await GetDataAsync($"{_baseUrl}/api/collections/byId/{collectionId}", cancellationToken));
        }

        public async Task<CollectionsManagement.Models.Collection> GetCollectionAsync(string collectionName, CancellationToken cancellationToken = default(CancellationToken))
        {
           return _jsonSerializationService.Deserialize<CollectionsManagement.Models.Collection>(
                await GetDataAsync($"{_baseUrl}/api/collections/name/{collectionName}", cancellationToken));
        }

        public async Task<IEnumerable<ReturnPeriod>> GetReturnPeriodsForCollection(int collectionId, CancellationToken cancellationToken = default(CancellationToken))
        {
            var data = _jsonSerializationService.Deserialize<IEnumerable<CollectionsManagement.Models.ReturnPeriod>>(
                await GetDataAsync($"{_baseUrl}/api/returnperiod/collectionId/{collectionId}", cancellationToken));

            var now = _dateTimeProvider.GetNowUtc();

            return data
                .Select(d => new ReturnPeriod(
                    d.ReturnPeriodId,
                    $"R{d.PeriodNumber:00}",
                    d.StartDateTimeUtc,
                    d.EndDateTimeUtc,
                    (d.StartDateTimeUtc <= now && d.EndDateTimeUtc >= now) || d.StartDateTimeUtc > now))
                .ToList();
        }

        public async Task<ReturnPeriod> GetReturnPeriod(int id, CancellationToken cancellationToken = default(CancellationToken))
        {
            var data = _jsonSerializationService.Deserialize<CollectionsManagement.Models.ReturnPeriod>(
                await GetDataAsync($"{_baseUrl}/api/returnperiod/{id}", cancellationToken));

            return new ReturnPeriod(data.ReturnPeriodId, $"R{data.PeriodNumber:00}", data.StartDateTimeUtc, data.EndDateTimeUtc)
            {
                CollectionName = data.CollectionName,
                CollectionId = data.CollectionId
            };
        }

        public async Task<bool> UpdateReturnPeriod(ReturnPeriod returnPeriod, CancellationToken cancellationToken = default(CancellationToken))
        {
            var returnPeriodDto = new CollectionsManagement.Models.ReturnPeriod()
            {
                ReturnPeriodId = returnPeriod.ReturnPeriodId,
                StartDateTimeUtc = returnPeriod.OpenDate,
                EndDateTimeUtc = returnPeriod.CloseDate
            };

            return (await SendDataAsyncRawResponse($"{_baseUrl}/api/returnperiod/update", returnPeriodDto, cancellationToken)).IsSuccess;
        }
    }
}
