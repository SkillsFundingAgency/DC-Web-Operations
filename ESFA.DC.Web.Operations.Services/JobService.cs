﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Jobs.Model;
using ESFA.DC.Jobs.Model.Enums;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Web.Operations.Interfaces;
using ESFA.DC.Web.Operations.Settings.Models;

namespace ESFA.DC.Web.Operations.Services
{
    public class JobService : BaseHttpClientService, IJobService
    {
        private readonly string _baseUrl;

        public JobService(
            IRouteFactory routeFactory,
            IJsonSerializationService jsonSerializationService,
            ApiSettings apiSettings,
            HttpClient httpClient)
            : base(routeFactory, jsonSerializationService, httpClient)
        {
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

            var response = await SendDataAsync($"{_baseUrl}/api/job", job, cancellationToken);
            long.TryParse(response, out var result);

            return result;
        }

        public async Task<JobStatusType> GetJobStatus(long jobId, CancellationToken cancellationToken = default)
        {
            var data = await GetDataAsync($"{_baseUrl}/api/job/{jobId}/status", cancellationToken);
            return _jsonSerializationService.Deserialize<JobStatusType>(data);
        }

        public async Task<SubmittedJob> GetJob(long ukprn, long jobId, CancellationToken cancellationToken = default)
        {
            var data = await GetDataAsync($"{_baseUrl}/api/job/{ukprn}/{jobId}", cancellationToken);
            return _jsonSerializationService.Deserialize<SubmittedJob>(data);
        }

        public async Task<IEnumerable<SubmittedJob>> GetLatestJobForReferenceDataCollectionsAsync(string collectionType, CancellationToken cancellationToken)
        {
            var data = await GetDataAsync($"{_baseUrl}/api/job/{collectionType}/latestByType", cancellationToken);

            return data == null ? null : _jsonSerializationService.Deserialize<IEnumerable<SubmittedJob>>(data);
        }

        public async Task<SubmittedJob> GetLatestJobForCollectionAsync(string collection, CancellationToken cancellationToken)
        {
            var data = await GetDataAsync($"{_baseUrl}/api/job/{collection}/latest", cancellationToken);

            return data == null ? null : _jsonSerializationService.Deserialize<SubmittedJob>(data);
        }

        public async Task<SubmittedJob> GetLatestJobAsync(long ukprn, string collectionName, CancellationToken cancellationToken)
        {
            var data = await GetDataAsync($"{_baseUrl}/{ukprn}/{collectionName}/latest", cancellationToken);
            if (string.IsNullOrEmpty(data))
            {
                return null;
            }

            return _jsonSerializationService.Deserialize<SubmittedJob>(data);
        }
    }
}