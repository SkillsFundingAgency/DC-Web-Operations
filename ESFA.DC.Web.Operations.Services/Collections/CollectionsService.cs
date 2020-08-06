using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.Jobs.Model;
using ESFA.DC.Jobs.Model.Enums;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Web.Operations.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.Collections;
using ESFA.DC.Web.Operations.Models.Collection;
using ESFA.DC.Web.Operations.Settings.Models;
using ReturnPeriod = ESFA.DC.Web.Operations.Models.Collection.ReturnPeriod;

namespace ESFA.DC.Web.Operations.Services.Collections
{
    public class CollectionsService : BaseHttpClientService, ICollectionsService
    {
        private readonly string _baseUrl;
        private readonly string[] _collectionsTypesToExclude = { "REF", "PE", "FRM", "OP" };
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly ILogger _logger;

        public CollectionsService(
            IRouteFactory routeFactory,
            IJsonSerializationService jsonSerializationService,
            ApiSettings apiSettings,
            HttpClient httpClient,
            IDateTimeProvider dateTimeProvider,
            ILogger logger)
            : base(routeFactory, jsonSerializationService, httpClient)
        {
            _baseUrl = apiSettings.JobManagementApiBaseUrl;
            _dateTimeProvider = dateTimeProvider;
            _logger = logger;
        }

        public async Task<IEnumerable<CollectionSummary>> GetAllCollectionSummariesForYear(int year, CancellationToken cancellationToken = default(CancellationToken))
        {
            var data = _jsonSerializationService.Deserialize<IEnumerable<CollectionsManagement.Models.Collection>>(
                await GetDataAsync($"{_baseUrl}/api/collections/for-year/{year}", cancellationToken));

            return data.Where(c => !_collectionsTypesToExclude.Contains(c.CollectionType))
                .Select(collection => new CollectionSummary
                {
                    CollectionId = collection.CollectionId,
                    CollectionYear = year,
                    CollectionName = collection.CollectionTitle,
                    IsCollectionOpen = collection.IsOpen,
                    LastPeriodClosedDate = collection.LastPeriodClosedDate,
                    LastPeriodNumber = collection.LastPeriodNumber,
                    NextPeriodNumber = collection.NextPeriodNumber,
                    NextPeriodOpenDate = collection.NextPeriodOpenDateTimeUtc,
                    OpenPeriodNumber = collection.OpenPeriodNumber,
                    OpenPeriodCloseDate = collection.OpenPeriodCloseDate
                })
                .ToList();
        }

        public async Task<IEnumerable<int>> GetAvailableCollectionYears(CancellationToken cancellationToken = default(CancellationToken))
        {
            return _jsonSerializationService.Deserialize<IEnumerable<int>>(
                await GetDataAsync($"{_baseUrl}/api/collections/available-years", cancellationToken));
        }

        public async Task<IEnumerable<int>> GetCollectionYearsByType(string collectionType, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _jsonSerializationService.Deserialize<IEnumerable<int>>(
                await GetDataAsync($"{_baseUrl}/api/collections/years/{collectionType}", cancellationToken));
        }

        public async Task<IEnumerable<CollectionsManagement.Models.Collection>> GetCollectionsByType(string collectionType, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _jsonSerializationService.Deserialize<IEnumerable<CollectionsManagement.Models.Collection>>(
                await GetDataAsync($"{_baseUrl}/api/collections/type/{collectionType}", cancellationToken));
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

        public async Task<IEnumerable<FileUploadJob>> GetCollectionJobs(string collectionName, CancellationToken cancellationToken)
        {
            var data = await GetDataAsync($"{_baseUrl}/api/job/all-periods/{collectionName}/{(short)JobStatusType.Ready}", cancellationToken);
            return data != null ? _jsonSerializationService.Deserialize<IEnumerable<FileUploadJob>>(data) : new List<FileUploadJob>();
        }

        public async Task<CollectionsManagement.Models.Collection> GetCollectionFromName(string collectionName, CancellationToken cancellationToken = default(CancellationToken))
        {
            var data = _jsonSerializationService.Deserialize<CollectionsManagement.Models.Collection>(
                await GetDataAsync($"{_baseUrl}/api/collections/name/{collectionName}", cancellationToken));

            return new CollectionsManagement.Models.Collection()
            {
                CollectionTitle = data.CollectionTitle,
                CollectionId = data.CollectionId,
                ProcessingOverride = data.ProcessingOverride
            };
        }

        public async Task<bool> FailJob(int jobId, CancellationToken cancellationToken)
        {
            var dto = new JobStatusDto()
            {
                JobId = jobId,
                ContinueToFailJob = true,
                JobStatus = (int)JobStatusType.Failed,
            };

            var response = await SendDataAsyncRawResponse($"{_baseUrl}/api/job/status", dto, cancellationToken);

            if (!response.IsSuccess)
            {
                _logger.LogError($"Web Operations. Error occured failing job: {jobId}");
            }

            return response.IsSuccess;
        }

        public async Task<bool> SetCollectionProcessingOverride(int collectionId, bool? collectionProcessingOverrideStatus, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _jsonSerializationService.Deserialize<bool>(
                await SendAsync($"{_baseUrl}/api/collections/set-collection-processing-override/{collectionId}/{collectionProcessingOverrideStatus}", cancellationToken));
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
                    _dateTimeProvider.ConvertUtcToUk(d.StartDateTimeUtc),
                    _dateTimeProvider.ConvertUtcToUk(d.EndDateTimeUtc),
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
