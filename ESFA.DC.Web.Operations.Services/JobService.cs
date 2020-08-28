using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.Jobs.Model;
using ESFA.DC.Jobs.Model.Enums;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Web.Operations.Interfaces;
using ESFA.DC.Web.Operations.Settings.Models;

namespace ESFA.DC.Web.Operations.Services
{
    public class JobService : IJobService
    {
        private readonly IHttpClientService _httpClientService;
        private readonly string _baseUrl;

        public JobService(
            ApiSettings apiSettings,
            IHttpClientService httpClientService)
        {
            _httpClientService = httpClientService;
            _baseUrl = apiSettings.JobManagementApiBaseUrl;
        }

        public async Task<long> SubmitJob(Models.Job.JobSubmission submittedJobSubmission, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(submittedJobSubmission?.FileName))
            {
                throw new ArgumentException("submission message should have file name");
            }

            var job = new FileUploadJob
            {
                Ukprn = submittedJobSubmission.Ukprn,
                Priority = 1,
                Status = JobStatusType.Ready,
                CreatedBy = submittedJobSubmission.SubmittedBy,
                FileName = submittedJobSubmission.FileName,
                IsFirstStage = true,
                StorageReference = submittedJobSubmission.StorageReference,
                FileSize = submittedJobSubmission.FileSizeBytes,
                CollectionName = submittedJobSubmission.CollectionName,
                PeriodNumber = submittedJobSubmission.Period,
                NotifyEmail = submittedJobSubmission.NotifyEmail,
                TermsAccepted = submittedJobSubmission.TermsAccepted,
            };

            var response = await _httpClientService.SendDataAsync($"{_baseUrl}/api/job", job, cancellationToken);
            long.TryParse(response, out var result);

            return result;
        }

        public async Task<JobStatusType> GetJobStatus(long jobId, CancellationToken cancellationToken = default)
        {
            return await _httpClientService.GetAsync<JobStatusType>($"{_baseUrl}/api/job/{jobId}/status", cancellationToken);
        }

        public async Task<SubmittedJob> GetJob(long ukprn, long jobId, CancellationToken cancellationToken = default)
        {
            return await _httpClientService.GetAsync<SubmittedJob>($"{_baseUrl}/api/job/{ukprn}/{jobId}", cancellationToken);
        }

        public async Task<IEnumerable<SubmittedJob>> GetLatestJobForReferenceDataCollectionsAsync(string collectionType, CancellationToken cancellationToken)
        {
            return await _httpClientService.GetAsync<IEnumerable<SubmittedJob>>($"{_baseUrl}/api/job/{collectionType}/latestByType", cancellationToken);
        }

        public async Task<SubmittedJob> GetLatestJobForCollectionAsync(string collection, CancellationToken cancellationToken)
        {
            return await _httpClientService.GetAsync<SubmittedJob>($"{_baseUrl}/api/job/{collection}/latest", cancellationToken);
        }

        public async Task<SubmittedJob> GetLatestJobAsync(long ukprn, string collectionName, CancellationToken cancellationToken)
        {
            return await _httpClientService.GetAsync<SubmittedJob>($"{_baseUrl}/{ukprn}/{collectionName}/latest", cancellationToken);
        }
    }
}