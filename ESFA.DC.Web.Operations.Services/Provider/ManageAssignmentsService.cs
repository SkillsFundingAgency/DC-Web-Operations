using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.CollectionsManagement.Models;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.Provider;
using ESFA.DC.Web.Operations.Models.Collection;
using ESFA.DC.Web.Operations.Settings.Models;
using ESFA.DC.Web.Operations.Utils;
using MoreLinq;
using CollectionType = ESFA.DC.CollectionsManagement.Models.Enums.CollectionType;

namespace ESFA.DC.Web.Operations.Services.Provider
{
    public class ManageAssignmentsService : ProviderBaseService, IManageAssignmentsService
    {
        private const int ReturnPeriodCollectionOpenVarianceInMonths = 2;
        private const int FundingClaimCollectionOpenVarianceInDays = 14;

        private readonly string[] _jobManagementCollectionsTypesToExclude = { "REF", "PE", "OP", "COVID19", "COVIDR", "FRM", "FC", "ALLF" };
        private readonly ILogger _logger;
        private readonly string _jobManagementBaseUrl;
        private readonly string _fundingClaimsBaseUrl;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IHttpClientService _httpClientService;

        public ManageAssignmentsService(
            ILogger logger,
            IDateTimeProvider dateTimeProvider,
            ApiSettings apiSettings,
            IHttpClientService httpClientService)
            : base(apiSettings, dateTimeProvider, httpClientService)
        {
            _logger = logger;
            _jobManagementBaseUrl = apiSettings.JobManagementApiBaseUrl;
            _fundingClaimsBaseUrl = apiSettings.FundingClaimsApiBaseUrl;
            _dateTimeProvider = dateTimeProvider;
            _httpClientService = httpClientService;
        }

        public async Task<IEnumerable<CollectionAssignment>> GetAvailableCollectionsAsync(CancellationToken cancellationToken)
        {
            var openReturnPeriodCollections = GetOpenCollectionsWithReturnPeriods(cancellationToken);
            var openFundingClaimCollections = GetOpenFundingClaimCollections(cancellationToken);

            await Task.WhenAll(openReturnPeriodCollections, openFundingClaimCollections);

            return openReturnPeriodCollections.Result.Concat(openFundingClaimCollections.Result);
        }

        public async Task<bool> UpdateProviderAssignmentsAsync(long ukprn, ICollection<CollectionAssignment> assignments, CancellationToken cancellationToken)
        {
            _logger.LogInfo($"Entered UpdateProviderAssignments - Web Operations. Total number of updates:{assignments.Count}");

            var organisationToUpdate = new List<OrganisationCollection>();
            assignments
                .Where(w => !w.ToBeDeleted)
                .ForEach(a => organisationToUpdate.Add(new OrganisationCollection
                {
                    CollectionId = a.CollectionId,
                    StartDate = a.StartDate ?? DateTime.MinValue,
                    EndDate = a.EndDate ?? Constants.MaxDateTime
                }));

            var organisationToDelete = new List<OrganisationCollection>();
            assignments
                .Where(w => w.ToBeDeleted)
                .ForEach(a => organisationToDelete.Add(new OrganisationCollection
                {
                    CollectionId = a.CollectionId
                }));

            try
            {
                await _httpClientService.SendDataAsync($"{_jobManagementBaseUrl}/api/org/assignments/delete/{ukprn}", organisationToDelete, cancellationToken);
                _logger.LogInfo($"Entered UpdateProviderAssignments - Web Operations. Successfully deleted:{organisationToDelete.Count}");

                await _httpClientService.SendDataAsync($"{_jobManagementBaseUrl}/api/org/assignments/update/{ukprn}", organisationToUpdate, cancellationToken);
                _logger.LogInfo($"Entered UpdateProviderAssignments - Web Operations. Successfully updated:{organisationToUpdate.Count}");
                return true;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError("Error Updating Organisation Collections", ex);
                return false;
            }
        }

        private async Task<IEnumerable<CollectionAssignment>> GetOpenCollectionsWithReturnPeriods(CancellationToken cancellationToken)
        {
            var nowUtc = _dateTimeProvider.GetNowUtc();
            var startDateUtc = nowUtc.AddMonths(ReturnPeriodCollectionOpenVarianceInMonths);
            var endDateUtc = nowUtc.AddMonths(-ReturnPeriodCollectionOpenVarianceInMonths);

            var collections = await _httpClientService.GetAsync<IEnumerable<Collection>>($"{_jobManagementBaseUrl}/api/collections/ByDateRange/{startDateUtc:o}/{endDateUtc:o}", cancellationToken);

            return collections
                .Where(c => !_jobManagementCollectionsTypesToExclude.Contains(c.CollectionType))
                .Select(s => new CollectionAssignment
                {
                    CollectionId = s.CollectionId,
                    Name = s.CollectionTitle,
                    DisplayOrder = SetDisplayOrder((CollectionType)Enum.Parse(typeof(CollectionType), s.CollectionType), s.CollectionTitle),
                });
        }

        private async Task<IEnumerable<CollectionAssignment>> GetOpenFundingClaimCollections(CancellationToken cancellationToken)
        {
            var nowUtc = _dateTimeProvider.GetNowUtc();
            var endDateUtc = nowUtc.AddDays(-FundingClaimCollectionOpenVarianceInDays);

            var collections = await _httpClientService.GetAsync<IEnumerable<FundingClaimsCollection>>($"{_fundingClaimsBaseUrl}/collection/ByDateRange/{nowUtc:o}/{endDateUtc:o}", cancellationToken);

            if (collections != null)
            {
                return collections.Select(s => new CollectionAssignment
                {
                    CollectionId = s.CollectionId,
                    Name = s.CollectionName,
                    DisplayOrder = SetDisplayOrder(CollectionType.FC, s.CollectionName),
                });
            }

            return new List<CollectionAssignment>();
        }
    }
}