using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.Collections;
using ESFA.DC.Web.Operations.Models.Collection;
using ESFA.DC.Web.Operations.Settings.Models;
using Collection = ESFA.DC.CollectionsManagement.Models.Collection;

namespace ESFA.DC.Web.Operations.Services.Collections
{
    public class CollectionsService : BaseHttpClientService, ICollectionsService
    {
        private readonly string _baseUrl;
        private readonly string[] _collectionsTypesToExclude = { "REF", "PE" };

        public CollectionsService(
            IJsonSerializationService jsonSerializationService,
            ApiSettings apiSettings,
            HttpClient httpClient)
            : base(jsonSerializationService, httpClient)
        {
            _baseUrl = apiSettings.JobManagementApiBaseUrl;
        }

        public async Task<IEnumerable<CollectionSummary>> GetAllCollectionSummariesForYear(int year, CancellationToken cancellationToken = default(CancellationToken))
        {
            var data = _jsonSerializationService.Deserialize<IEnumerable<Collection>>(
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
    }
}
