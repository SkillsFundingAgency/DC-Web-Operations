using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Features.Indexed;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.FileService.Interface;
using ESFA.DC.Jobs.Model;
using ESFA.DC.PeriodEnd.Models.Dtos;
using ESFA.DC.Web.Operations.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.Publication;
using ESFA.DC.Web.Operations.Settings.Models;
using MoreLinq;

namespace ESFA.DC.Web.Operations.Services.Frm
{
    public class ReportsPublicationService : IReportsPublicationService
    {
        private readonly IFileService _fileService;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IHttpClientService _httpClientService;
        private readonly string _jobApiUrl;
        private readonly string _baseJobApiUrl;

        public ReportsPublicationService(
            IIndex<PersistenceStorageKeys, IFileService> fileService,
            IDateTimeProvider dateTimeProvider,
            ApiSettings apiSettings,
            IHttpClientService httpClientService)
        {
            _fileService = fileService[PersistenceStorageKeys.DctAzureStorage];
            _baseJobApiUrl = $"{apiSettings.JobManagementApiBaseUrl}/api";
            _jobApiUrl = $"{apiSettings.JobManagementApiBaseUrl}/api/job";
            _dateTimeProvider = dateTimeProvider;
            _httpClientService = httpClientService;
        }

        public async Task<int> GetFrmStatusAsync(long? jobId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (!jobId.HasValue)
            {
                throw new Exception("Missing 'jobId' parameter");
            }

            string url = $"{_jobApiUrl}/{jobId}/status";
            var response = await _httpClientService.GetDataAsync(url, cancellationToken);
            int.TryParse(response, out var result);
            return result;
        }

        public async Task<Models.Publication.JobDetails> GetFileSubmittedDetailsAsync(long? jobId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (!jobId.HasValue)
            {
                throw new Exception("Missing 'jobId' parameter");
            }

            string url = $"{_jobApiUrl}/0/{jobId}";
            var jobinfo = await _httpClientService.GetAsync<FileUploadJob>(url, cancellationToken);

            return new Models.Publication.JobDetails() {
                CollectionYear = jobinfo.CollectionYear,
                StorageReference = jobinfo.StorageReference,
                JobId = jobinfo.JobId,
                PeriodNumber = jobinfo.PeriodNumber,
                DateTimeSubmitted = _dateTimeProvider.ConvertUtcToUk(jobinfo.DateTimeCreatedUtc),
                CollectionName = jobinfo.CollectionName,
                CollectionPrefix = jobinfo.CollectionName.Substring(0, 3)
            };
        }

        public async Task<long> RunValidationAsync(string collectionName, string folderKey, int periodNumber, string userName, CancellationToken cancellationToken = default(CancellationToken))
        {
            var job = new ReportsPublicationJob()
            {
                CollectionName = collectionName,
                Status = Jobs.Model.Enums.JobStatusType.Ready,
                JobId = 0,
                SourceFolderKey = folderKey,
                PeriodNumber = periodNumber,
                CreatedBy = userName,
                //SourceContainerName = $"{collectionName.Substring(0, 3)}{coll}"
            };

            string url = $"{_jobApiUrl}/publication/validate";
            var result = await _httpClientService.SendDataAsync(url, job, cancellationToken);

            long.TryParse(result, out var jobId);

            return jobId;
        }

        public async Task<long> RunPublishAsync(long jobId, CancellationToken cancellationToken = default(CancellationToken))
        {
            JobStatusDto statusDto = new JobStatusDto()
            {
                JobId = jobId,
                JobStatus = 1
            };

            string url = $"{_jobApiUrl}/1";

            var result = _httpClientService.SendDataAsync(url, statusDto, cancellationToken);

            return jobId;
        }

        public async Task PublishSldAsync(long jobId, CancellationToken cancellationToken = default(CancellationToken))
        {
            string url = $"{_jobApiUrl}/publication/mark-as-published/{jobId}";
            await _httpClientService.SendAsync(url, cancellationToken);
        }

        public async Task UnpublishSldAsync(int periodNumber, int yearPeriod, CancellationToken cancellationToken = default(CancellationToken))
        {
            string path = $"{yearPeriod}/{periodNumber}";
            string url = $"{_jobApiUrl}/publication/mark-as-unpublished/{path}";
            await _httpClientService.SendAsync(url, cancellationToken);
        }

        public async Task UnpublishSldDeleteFolderAsync(string containerName, int period, CancellationToken cancellationToken = default(CancellationToken))
        {
            string folder = $"R{period.ToString("D2")}";
            await _fileService.DeleteFolderAsync(folder, containerName, cancellationToken);
        }

        public async Task<IEnumerable<PeriodEndCalendarYearAndPeriodModel>> GetFrmReportsDataAsync(string collectionName, CancellationToken cancellationToken = default(CancellationToken))
        {
            string url = $"{_jobApiUrl}/publication/published-periods/{collectionName}";
            var result = await _httpClientService.GetAsync<List<PeriodEndCalendarYearAndPeriodModel>>(url, cancellationToken);
            return result.OrderBy(x => x.CollectionYear).ThenBy(y => y.PeriodNumber);
        }

        public async Task<IEnumerable<int>> GetLastTwoCollectionYearsAsync(string collectionType, CancellationToken cancellationToken = default(CancellationToken))
        {
            string url = $"{_baseJobApiUrl}/collections/years/{collectionType}";
            var result = await _httpClientService.GetAsync<List<int>>(url, cancellationToken);
            return result.OrderBy(y => y).TakeLast(2);
         }
    }
}