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
    public abstract class ProviderBaseService
    {
        private readonly string _baseUrl;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IHttpClientService _httpClientService;

        public ProviderBaseService(
            ApiSettings apiSettings,
            IDateTimeProvider dateTimeProvider,
            IHttpClientService httpClientService)
        {
            _baseUrl = apiSettings.JobManagementApiBaseUrl;
            _dateTimeProvider = dateTimeProvider;
            _httpClientService = httpClientService;
        }

        public async Task<Models.Provider.Provider> GetProviderAsync(long ukprn, CancellationToken cancellationToken)
        {
            var data = await _httpClientService.GetAsync<ProviderDetail>($"{_baseUrl}/api/org/{ukprn}", cancellationToken);

            return new Models.Provider.Provider(data.Name, data.Ukprn, data.Upin, data.IsMCA);
        }

        public async Task<IEnumerable<CollectionAssignment>> GetProviderAssignmentsAsync(long ukprn, CancellationToken cancellationToken)
        {
            var response = await _httpClientService.GetAsync<IEnumerable<OrganisationCollection>>($"{_baseUrl}/api/org/assignments/{ukprn}", cancellationToken);

            var date = _dateTimeProvider.GetNowUtc();
            var earliestStartDate = date.AddMonths(2);
            var expiredEndDate = date.AddMonths(-2);

            return response
                .Where(pa => pa.StartDate <= earliestStartDate && pa.EndDate >= expiredEndDate)
                .Select(p => new CollectionAssignment
                {
                    CollectionId = p.CollectionId,
                    Name = p.CollectionName,
                    StartDate = _dateTimeProvider.ConvertUtcToUk(p.StartDate),
                    EndDate = _dateTimeProvider.ConvertUtcToUk(p.EndDate.GetValueOrDefault()),
                    DisplayOrder = SetDisplayOrder(p.CollectionType, p.CollectionName),
                    ToBeDeleted = false
                });
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
