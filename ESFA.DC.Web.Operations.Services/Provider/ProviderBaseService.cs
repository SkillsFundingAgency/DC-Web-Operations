using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.CollectionsManagement.Models;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.Jobs.Model;
using ESFA.DC.Web.Operations.Interfaces;
using ESFA.DC.Web.Operations.Models.Collection;
using ESFA.DC.Web.Operations.Services.Extensions;
using ESFA.DC.Web.Operations.Settings.Models;
using MoreLinq.Extensions;
using CollectionType = ESFA.DC.CollectionsManagement.Models.Enums.CollectionType;

namespace ESFA.DC.Web.Operations.Services.Provider
{
    public abstract class ProviderBaseService
    {
        private readonly string _jobManagementBaseUrl;
        private readonly string _fundingClaimsBaseUrl;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IHttpClientService _httpClientService;

        public ProviderBaseService(
            ApiSettings apiSettings,
            IDateTimeProvider dateTimeProvider,
            IHttpClientService httpClientService)
        {
            _jobManagementBaseUrl = apiSettings.JobManagementApiBaseUrl;
            _fundingClaimsBaseUrl = apiSettings.FundingClaimsApiBaseUrl;
            _dateTimeProvider = dateTimeProvider;
            _httpClientService = httpClientService;
        }

        public async Task<Models.Provider.Provider> GetProviderAsync(long ukprn, CancellationToken cancellationToken)
        {
            var data = await _httpClientService.GetAsync<ProviderDetail>($"{_jobManagementBaseUrl}/api/org/{ukprn}", cancellationToken);

            return new Models.Provider.Provider(data.Name, data.Ukprn, data.Upin, data.IsMCA);
        }

        public async Task<IEnumerable<CollectionAssignment>> GetProviderAssignmentsAsync(long ukprn, CancellationToken cancellationToken)
        {
            var response = await _httpClientService.GetAsync<IEnumerable<OrganisationCollection>>($"{_jobManagementBaseUrl}/api/org/assignments/{ukprn}", cancellationToken);

            var collectionAssignments = response.Select(a => new CollectionAssignment
            {
                CollectionId = a.CollectionId,
                Name = a.CollectionName,
                StartDate = a.StartDate,
                EndDate = a.EndDate,
                DisplayOrder = SetDisplayOrder(a.CollectionType, a.CollectionName),
                ToBeDeleted = false
            }).ToList();

            return collectionAssignments;
        }

        public async Task<IEnumerable<CollectionAssignment>> GetActiveProviderAssignmentsAsync(long ukprn, CancellationToken cancellationToken)
        {
            const int minimumVarianceInMonths = -2;
            const int maximumVarianceInMonths = 2;

            var response = await _httpClientService.GetAsync<IEnumerable<OrganisationCollection>>($"{_jobManagementBaseUrl}/api/org/assignments/{ukprn}", cancellationToken);
            var dateTimeNow = _dateTimeProvider.GetNowUtc();
            var isOpenCollectionRequests = new Dictionary<OrganisationCollection, Task<bool>>();

            foreach (var assignment in response)
            {
                if (assignment.CollectionType == CollectionType.FC)
                {
                    isOpenCollectionRequests.Add(assignment, _httpClientService.GetAsync<bool>($"{_fundingClaimsBaseUrl}/collection/isOpenWithVariance/{assignment.CollectionName}/{dateTimeNow.ToString("s", CultureInfo.InvariantCulture)}/{minimumVarianceInMonths}/{maximumVarianceInMonths}", cancellationToken));
                }
                else
                {
                    isOpenCollectionRequests.Add(assignment, _httpClientService.GetAsync<bool>($"{_jobManagementBaseUrl}/api/collections/isOpenWithVariance/{assignment.CollectionId}/{dateTimeNow.ToString("s", CultureInfo.InvariantCulture)}/{minimumVarianceInMonths}/{maximumVarianceInMonths}", cancellationToken));
                }
            }

            await Task.WhenAll(isOpenCollectionRequests.Values);

            return (from request in isOpenCollectionRequests
                where request.Value.Result
                select new CollectionAssignment
                {
                    CollectionId = request.Key.CollectionId,
                    Name = request.Key.CollectionName,
                    StartDate = _dateTimeProvider.ConvertUtcToUk(request.Key.StartDate),
                    EndDate = request.Key.EndDate.HasValue ? _dateTimeProvider.ConvertUtcToUk(request.Key.EndDate.Value) : (DateTime?)null,
                    DisplayOrder = SetDisplayOrder(request.Key.CollectionType, request.Key.CollectionName),
                    ToBeDeleted = false
                }).ToList();
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
