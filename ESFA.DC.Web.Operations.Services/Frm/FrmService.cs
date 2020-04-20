namespace ESFA.DC.Web.Operations.Services.Frm
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Autofac.Features.AttributeFilters;
    using ESFA.DC.FileService.Interface;
    using ESFA.DC.Jobs.Model;
    using ESFA.DC.Logging.Interfaces;
    using ESFA.DC.PeriodEnd.Models.Dtos;
    using ESFA.DC.Serialization.Interfaces;
    using ESFA.DC.Web.Operations.Interfaces.Frm;
    using ESFA.DC.Web.Operations.Settings.Models;
    using ESFA.DC.Web.Operations.Utils;
    using MoreLinq;

    public class FrmService : BaseHttpClientService, IFrmService
    {
        private readonly ILogger _logger;
        private readonly IFileService _fileService;
        private readonly HttpClient _httpClient;
        private readonly string _jobApiUrl;
        private readonly string _periodEndJobApiUrl;
        private readonly string _baseJobApiUrl;

        public FrmService(
            IJsonSerializationService jsonSerializationService,
            IFileService fileService,
            ApiSettings apiSettings,
            HttpClient httpClient)
            : base(jsonSerializationService, httpClient)
        {
            _fileService = fileService;
            _baseJobApiUrl = $"{apiSettings.JobManagementApiBaseUrl}/api";
            _jobApiUrl = $"{apiSettings.JobManagementApiBaseUrl}/api/job";
            _periodEndJobApiUrl = $"{apiSettings.JobManagementApiBaseUrl}/api/period-end/frm-reports";
            _httpClient = httpClient;
        }

        public async Task<int> GetFrmStatusAsync(long? jobId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (!jobId.HasValue)
            {
                throw new Exception("Missing 'jobId' parameter");
            }

            string url = $"{_jobApiUrl}/{jobId}/status";
            var response = await GetDataAsync(url, cancellationToken);
            int.TryParse(response, out var result);
            return result;
        }

        public async Task<DateTime?> GetFileSubmittedDateAsync(long? jobId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (!jobId.HasValue)
            {
                throw new Exception("Missing 'jobId' parameter");
            }

            string url = $"{_jobApiUrl}/0/{jobId}";
            var jobinfojson = await GetDataAsync(url, cancellationToken);
            var jobinfo = _jsonSerializationService.Deserialize<FileUploadJob>(jobinfojson);
            return jobinfo.DateTimeSubmittedUtc;
        }

        public async Task<long> RunValidationAsync(string containerName, string folderKey, int periodNumber, string storageReference, string userName, CancellationToken cancellationToken = default(CancellationToken))
        {
            string collectionName = Constants.FrmReportCollectionName;
            FrmReportsJob job = new FrmReportsJob()
            {
                CollectionName = collectionName,
                Status = Jobs.Model.Enums.JobStatusType.Ready,
                JobId = 0,
                SourceContainerName = containerName,
                SourceFolderKey = folderKey,
                PeriodNumber = periodNumber,
                StorageReference = storageReference,
                CreatedBy = userName
            };

            string url = $"{_jobApiUrl}/frm/validate";
            var json = _jsonSerializationService.Serialize(job);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PostAsync(url, content, cancellationToken);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();
            long.TryParse(result, out var jobId);

            return jobId;
        }

        public async Task<long> RunPublishAsync(long jobId, CancellationToken cancellationToken = default(CancellationToken))
        {
            string collectionName = Constants.FrmReportCollectionName;

            JobStatusDto statusDto = new JobStatusDto()
            {
                JobId = jobId,
                JobStatus = 1
            };

            string url = $"{_jobApiUrl}/1";
            var json = _jsonSerializationService.Serialize(statusDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PostAsync(url, content, cancellationToken);
            response.EnsureSuccessStatusCode();

            return jobId;
        }

        public async Task PublishSldAsync(int collectionYear, int periodNumber, CancellationToken cancellationToken = default(CancellationToken))
        {
            string url = $"{_periodEndJobApiUrl}/{collectionYear}/{periodNumber}/publish";
            HttpResponseMessage response = await _httpClient.PostAsync(url, null, cancellationToken);
            response.EnsureSuccessStatusCode();

        }

        public async Task UnpublishSldAsync(string path, CancellationToken cancellationToken = default(CancellationToken))
        {
            string url = $"{_periodEndJobApiUrl}/{path}/unpublish";
            HttpResponseMessage response = await _httpClient.PostAsync(url, null, cancellationToken);
            response.EnsureSuccessStatusCode();
        }

        public async Task UnpublishSldDeleteFolderAsync(string containerName, string path, CancellationToken cancellationToken = default(CancellationToken))
        {
            _fileService.
        }

        public async Task<IEnumerable<PeriodEndCalendarYearAndPeriodModel>> GetFrmReportsDataAsync()
        {
            string url = $"{_periodEndJobApiUrl}/getfrmreportsdata";
            var response = await _httpClient.GetStringAsync(url);
            var unsortedJson = _jsonSerializationService.Deserialize<List<PeriodEndCalendarYearAndPeriodModel>>(response);
            return unsortedJson.OrderBy(x => x.CollectionYear).ThenBy(y => y.PeriodNumber);
        }

        public async Task<IEnumerable<int>> GetLastTwoCollectionYearsAsync(string collectionType)
        {
            string url = $"{_baseJobApiUrl}/collections/years/{collectionType}";
            var reponse = await _httpClient.GetStringAsync(url);
            var years = _jsonSerializationService.Deserialize<List<int>>(reponse);
            years.OrderBy(year => years);
            return years.TakeLast(2);
         }
    }
}