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
using ESFA.DC.Web.Operations.Services.Enums;
using ESFA.DC.Web.Operations.Settings.Models;
using ESFA.DC.Web.Operations.Utils;
using MoreLinq;
using CollectionType = ESFA.DC.CollectionsManagement.Models.Enums.CollectionType;

namespace ESFA.DC.Web.Operations.Services.Provider
{
    public class ManageAssignmentsService : ProviderBaseService, IManageAssignmentsService
    {
        private readonly string[] _jobManagementCollectionsTypesToExclude = { "REF", "PE", "OP", "COVID19", "COVIDR", "FRM", "FC" };
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
            var collectionYears = GetCollectionYears();

            var openReturnPeriodCollections = await GetOpenCollectionsWithReturnPeriods(collectionYears, cancellationToken);
            var openFundingClaimCollections = await GetOpenFundingClaimCollections(collectionYears, cancellationToken);

            return openReturnPeriodCollections.Concat(openFundingClaimCollections);
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

        private async Task<IEnumerable<CollectionAssignment>> GetOpenCollectionsWithReturnPeriods(List<int> collectionYears, CancellationToken cancellationToken)
        {
            var openCollections = new List<Collection>();

            foreach (var collectionYear in collectionYears)
            {
                var collections = await _httpClientService.GetAsync<IEnumerable<Collection>>($"{_jobManagementBaseUrl}/api/collections/for-year/{collectionYear}", cancellationToken);
                if (collections != null)
                {
                    collections = collections.Where(c => !_jobManagementCollectionsTypesToExclude.Contains(c.CollectionType));
                    collections = collections.Where(c => IsReturnPeriodCollectionWithinDateTolerance(c.StartDateTimeUtc, c.EndDateTimeUtc));
                    openCollections.AddRange(collections);
                }
            }

            return openCollections.Select(s => new CollectionAssignment
            {
                CollectionId = s.CollectionId,
                Name = s.CollectionTitle,
                DisplayOrder = SetDisplayOrder((CollectionType)Enum.Parse(typeof(CollectionType), s.CollectionType), s.CollectionTitle),
            });
        }

        private async Task<IEnumerable<CollectionAssignment>> GetOpenFundingClaimCollections(List<int> collectionYears, CancellationToken cancellationToken)
        {
            var openCollections = new List<FundingClaimsCollection>();

            foreach (var collectionYear in collectionYears)
            {
                var collections = await _httpClientService.GetAsync<IEnumerable<FundingClaimsCollection>>($"{_fundingClaimsBaseUrl}/collection/collectionYear/{collectionYear}", cancellationToken);
                if (collections != null)
                {
                    collections = collections?.Where(c => IsFundingClaimCollectionWithinDateTolerance(c.SubmissionCloseDateUtc));
                    openCollections.AddRange(collections);
                }
            }

            return openCollections.Select(s => new CollectionAssignment
            {
                CollectionId = s.CollectionId,
                Name = s.CollectionName,
                DisplayOrder = SetDisplayOrder(CollectionType.FC, s.CollectionName),
            });
        }

        private List<int> GetCollectionYears()
        {
            var collectionYears = new List<int>();

            var nowUtc = _dateTimeProvider.GetNowUtc();

            switch (nowUtc.Month)
            {
                case int n when n >= 1 && n <= 7:
                    collectionYears = FormatDateToCollectionYear(new[] { CollectionYearOption.CurrentYearMinusOne }, nowUtc);
                    break;
                case int n when n >= 8 && n <= 10:
                    collectionYears = FormatDateToCollectionYear(new[] { CollectionYearOption.CurrentYear, CollectionYearOption.CurrentYearMinusOne }, nowUtc);
                    break;
                case int n when n >= 11 && n <= 12:
                    collectionYears = FormatDateToCollectionYear(new[] { CollectionYearOption.CurrentYear }, nowUtc);
                    break;
            }

            return collectionYears;
        }

        private List<int> FormatDateToCollectionYear(CollectionYearOption[] options, DateTime nowUtc)
        {
            var currentYearMinusOne = $"{(nowUtc.Year - 1).ToString().Substring(2, 2)}{nowUtc.Year.ToString().Substring(2, 2)}";
            var currentYear = $"{nowUtc.Year.ToString().Substring(2, 2)}{(nowUtc.Year + 1).ToString().Substring(2, 2)}";

            var years = new List<int>();

            foreach (var option in options)
            {
                switch (option)
                {
                    case CollectionYearOption.CurrentYear:
                        years.Add(int.Parse(currentYear));
                        break;
                    case CollectionYearOption.CurrentYearMinusOne:
                        years.Add(int.Parse(currentYearMinusOne));
                        break;
                }
            }

            return years;
        }

        private bool IsReturnPeriodCollectionWithinDateTolerance(DateTime? startDateUtc, DateTime? endDateUtc)
        {
            var now = _dateTimeProvider.GetNowUtc();

            if (startDateUtc.HasValue && endDateUtc.HasValue)
            {
                return startDateUtc.Value.AddMonths(-2) <= now && endDateUtc.Value.AddMonths(2) >= now;
            }

            return true;
        }

        private bool IsFundingClaimCollectionWithinDateTolerance(DateTime? endDateUtc)
        {
            if (endDateUtc.HasValue)
            {
                return endDateUtc.Value.AddDays(14) >= _dateTimeProvider.GetNowUtc();
            }

            return true;
        }
    }
}