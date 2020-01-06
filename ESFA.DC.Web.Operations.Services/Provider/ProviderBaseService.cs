using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.CollectionsManagement.Models;
using ESFA.DC.Jobs.Model;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Web.Operations.Models.Collection;
using ESFA.DC.Web.Operations.Services.Extensions;
using ESFA.DC.Web.Operations.Settings.Models;
using MoreLinq;
using CollectionType = ESFA.DC.CollectionsManagement.Models.Enums.CollectionType;

namespace ESFA.DC.Web.Operations.Services.Provider
{
    public class ProviderBaseService : BaseHttpClientService
    {
        private readonly ILogger _logger;
        private readonly string _baseUrl;

        public ProviderBaseService(
            ILogger logger,
            IJsonSerializationService jsonSerializationService,
            ApiSettings apiSettings,
            HttpClient httpClient)
            : base(jsonSerializationService, httpClient)
        {
            _logger = logger;
            _baseUrl = apiSettings.JobManagementApiBaseUrl;
        }

        public async Task<Models.Provider.Provider> GetProvider(long ukprn, CancellationToken cancellationToken = default(CancellationToken))
        {
            var data = _jsonSerializationService.Deserialize<ProviderDetail>(
                await GetDataAsync($"{_baseUrl}/api/org/{ukprn}", cancellationToken));

            return new Models.Provider.Provider(data.Name, data.Ukprn, data.Upin, data.IsMCA);
        }

        public async Task<IEnumerable<CollectionAssignment>> GetProviderAssignments(long ukprn, CancellationToken cancellationToken = default(CancellationToken))
        {
            var x = await GetDataAsync(_baseUrl + $"/api/org/assignments/{ukprn}", cancellationToken);

            var providerAssignments = _jsonSerializationService.Deserialize<IEnumerable<OrganisationCollection>>(x);

            var collectionAssignments = new List<CollectionAssignment>();

            providerAssignments.ForEach(pa => collectionAssignments.Add(new CollectionAssignment()
                { CollectionId = pa.CollectionId, Name = pa.CollectionName, StartDate = pa.StartDate, EndDate = pa.EndDate, DisplayOrder = SetDisplayOrder(pa.CollectionType, pa.CollectionName) }));

            return collectionAssignments;
        }

        public int SetDisplayOrder(CollectionsManagement.Models.Enums.CollectionType collectionType, string collectionName)
        {
            switch (collectionType)
            {
                case CollectionType.ILR:
                    return 1;
                case CollectionType.EAS:
                    return 2;
                case CollectionType.ESF:
                    return 3;
                case CollectionType.FC:
                {
                    if (collectionName.Contains("Final", StringComparison.OrdinalIgnoreCase))
                        return 4;
                    return collectionName.Contains("YearEnd", StringComparison.OrdinalIgnoreCase) ? 5 : 6;
                }

                case CollectionType.NCS:
                    return 7;
            }

            return 8;
        }
    }
}
