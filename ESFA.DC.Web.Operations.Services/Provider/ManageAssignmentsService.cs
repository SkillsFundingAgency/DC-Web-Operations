﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.CollectionsManagement.Models;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.Provider;
using ESFA.DC.Web.Operations.Models.Collection;
using ESFA.DC.Web.Operations.Services.Enums;
using ESFA.DC.Web.Operations.Settings.Models;
using MoreLinq;
using CollectionType = ESFA.DC.CollectionsManagement.Models.Enums.CollectionType;

namespace ESFA.DC.Web.Operations.Services.Provider
{
    public class ManageAssignmentsService : ProviderBaseService, IManageAssignmentsService
    {
        private readonly string[] _collectionsTypesToExclude = { "REF", "PE" };
        private readonly ILogger _logger;
        private readonly string _baseUrl;
        private readonly IDateTimeProvider _dateTimeProvider;

        public ManageAssignmentsService(
            ILogger logger,
            IDateTimeProvider dateTimeProvider,
            IJsonSerializationService jsonSerializationService,
            ApiSettings apiSettings,
            HttpClient httpClient)
            : base(logger, jsonSerializationService, apiSettings, httpClient)
        {
            _logger = logger;
            _baseUrl = apiSettings.JobManagementApiBaseUrl;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<IEnumerable<CollectionAssignment>> GetAvailableCollections(CancellationToken cancellationToken = default(CancellationToken))
        {
            var collectionYears = new List<int>();

            switch (_dateTimeProvider.GetNowUtc().Month)
            {
                case int n when n >= 1 && n <= 7:
                    collectionYears = FormatDateToCollectionYear(new[]
                        { CollectionYearOption.CurrentYearMinusOne });
                    break;
                case int n when n >= 8 && n <= 10:
                    collectionYears = FormatDateToCollectionYear(new[]
                        { CollectionYearOption.CurrentYear, CollectionYearOption.CurrentYearMinusOne });
                    break;
                case int n when n >= 11 && n <= 12:
                    collectionYears = FormatDateToCollectionYear(new[]
                        { CollectionYearOption.CurrentYear });
                    break;
            }

            var collections = new List<CollectionsManagement.Models.Collection>();

            foreach (var collectionYear in collectionYears)
            {
                collections.AddRange(_jsonSerializationService.Deserialize<IEnumerable<CollectionsManagement.Models.Collection>>
                    (await GetDataAsync(_baseUrl + $"/api/collections/for-year/{collectionYear}", cancellationToken)));
            }

            return collections
                .Where(w => !Array.Exists(_collectionsTypesToExclude, search => search.Contains(w.CollectionType.ToString())))
                .Select(s => new CollectionAssignment() { CollectionId = s.CollectionId, Name = s.CollectionTitle, DisplayOrder = SetDisplayOrder((CollectionType)Enum.Parse(typeof(CollectionType), s.CollectionType), s.CollectionTitle) })
                .ToList();
        }

        public async Task<bool> UpdateProviderAssignments(long ukprn, IEnumerable<CollectionAssignment> assignments, CancellationToken cancellationToken = default(CancellationToken))
        {
            var organisationCollection = new List<OrganisationCollection>();
            assignments
                .Where(w => w.StartDate.HasValue)
                .ForEach(a => organisationCollection.Add(new OrganisationCollection()
            {
                CollectionId = a.CollectionId,
                StartDate = a.StartDate.Value,
                EndDate = a.EndDate
            }));

            try
            {
                await SendDataAsync($"{_baseUrl}/api/org/assignments/update/{ukprn}", organisationCollection, cancellationToken);
                return true;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError("Error Updating Organisation Collections", ex);
                return false;
            }
        }

        private List<int> FormatDateToCollectionYear(CollectionYearOption[] options)
        {
            var currentYearMinusOne = $"{(_dateTimeProvider.GetNowUtc().Year - 1).ToString().Substring(2, 2)}{_dateTimeProvider.GetNowUtc().Year.ToString().Substring(2, 2)}";
            var currentYear = $"{_dateTimeProvider.GetNowUtc().Year.ToString().Substring(2, 2)}{(_dateTimeProvider.GetNowUtc().Year + 1).ToString().Substring(2, 2)}";

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
    }
}
