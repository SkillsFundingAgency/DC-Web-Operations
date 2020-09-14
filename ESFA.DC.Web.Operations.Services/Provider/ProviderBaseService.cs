using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.CollectionsManagement.Models;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.Jobs.Model;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Web.Operations.Interfaces;
using ESFA.DC.Web.Operations.Models.Collection;
using ESFA.DC.Web.Operations.Services.Extensions;
using ESFA.DC.Web.Operations.Settings.Models;
using CollectionType = ESFA.DC.CollectionsManagement.Models.Enums.CollectionType;

namespace ESFA.DC.Web.Operations.Services.Provider
{
    public abstract class ProviderBaseService : BaseHttpClientService
    {
        private readonly string _baseUrl;
        private readonly IDateTimeProvider _dateTimeProvider;

        public ProviderBaseService(
            IRouteFactory routeFactory,
            IJsonSerializationService jsonSerializationService,
            ApiSettings apiSettings,
            HttpClient httpClient,
            IDateTimeProvider dateTimeProvider)
            : base(routeFactory, jsonSerializationService, dateTimeProvider, httpClient)
        {
            _baseUrl = apiSettings.JobManagementApiBaseUrl;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<Models.Provider.Provider> GetProviderAsync(long ukprn, CancellationToken cancellationToken)
        {
            var data = _jsonSerializationService.Deserialize<ProviderDetail>(
                await GetDataAsync($"{_baseUrl}/api/org/{ukprn}", cancellationToken));

            return new Models.Provider.Provider(data.Name, data.Ukprn, data.Upin, data.IsMCA);
        }

        public async Task<IEnumerable<CollectionAssignment>> GetProviderAssignmentsAsync(long ukprn, CancellationToken cancellationToken)
        {
            var response = await GetDataAsync(_baseUrl + $"/api/org/assignments/{ukprn}", cancellationToken);

            var providerAssignments = _jsonSerializationService.Deserialize<IEnumerable<OrganisationCollection>>(response);

            var dateTimeNow = _dateTimeProvider.GetNowUtc();

            var collectionAssignments = new List<CollectionAssignment>();

            foreach (var assignment in providerAssignments)
            {
                var startDate = _dateTimeProvider.ConvertUtcToUk(assignment.StartDate);
                var endDate = _dateTimeProvider.ConvertUtcToUk(assignment.EndDate ?? dateTimeNow);

                var displayDateStart = startDate.AddMonths(-2);
                var displayDateEnd = endDate.AddMonths(2);

                if (dateTimeNow >= displayDateStart && dateTimeNow <= displayDateEnd)
                {
                    collectionAssignments.Add(new CollectionAssignment
                    {
                        CollectionId = assignment.CollectionId,
                        Name = assignment.CollectionName,
                        StartDate = startDate,
                        EndDate = endDate,
                        DisplayOrder = SetDisplayOrder(assignment.CollectionType, assignment.CollectionName),
                        ToBeDeleted = false
                    });
                }
            }

            return collectionAssignments;
        }

        public int SetDisplayOrder(CollectionType collectionType, string collectionName)
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
