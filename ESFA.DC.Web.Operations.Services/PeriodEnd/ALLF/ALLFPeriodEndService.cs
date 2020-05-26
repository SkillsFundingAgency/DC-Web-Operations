﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Jobs.Model;
using ESFA.DC.Jobs.Model.Enums;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Web.Operations.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.Collections;
using ESFA.DC.Web.Operations.Interfaces.PeriodEnd;
using ESFA.DC.Web.Operations.Interfaces.Storage;
using ESFA.DC.Web.Operations.Models;
using ESFA.DC.Web.Operations.Models.Job;
using ESFA.DC.Web.Operations.Settings.Models;
using Microsoft.AspNetCore.Http;

namespace ESFA.DC.Web.Operations.Services.PeriodEnd.ALLF
{
    public class ALLFPeriodEndService : BaseHttpClientService, IALLFPeriodEndService
    {
        private const string Api = "/api/period-end-allf/";

        private readonly IStorageService _storageService;
        private readonly IFileUploadJobMetaDataModelBuilderService _fileUploadJobMetaDataModelBuilderService;
        private readonly ICollectionsService _collectionService;
        private readonly IJobService _jobService;
        private readonly ILogger _logger;
        private readonly AzureStorageSection _azureStorageConfig;

        private readonly string _baseUrl;

        public ALLFPeriodEndService(
            IStorageService storageService,
            IFileUploadJobMetaDataModelBuilderService fileUploadJobMetaDataModelBuilderService,
            IJsonSerializationService jsonSerializationService,
            ICollectionsService collectionService,
            IJobService jobService,
            ILogger logger,
            AzureStorageSection azureStorageConfig,
            ApiSettings apiSettings,
            HttpClient httpClient)
            : base(jsonSerializationService, httpClient)
        {
            _storageService = storageService;
            _fileUploadJobMetaDataModelBuilderService = fileUploadJobMetaDataModelBuilderService;
            _collectionService = collectionService;
            _jobService = jobService;
            _logger = logger;
            _azureStorageConfig = azureStorageConfig;
            _baseUrl = apiSettings.JobManagementApiBaseUrl;
        }

        public async Task InitialisePeriodEndAsync(int year, int period, string collectionType, CancellationToken cancellationToken)
        {
            await SendAsync($"{_baseUrl}{Api}{year}/{period}/{collectionType}/initialise", cancellationToken);
        }

        public async Task StartPeriodEndAsync(int year, int period, string collectionType, CancellationToken cancellationToken = default(CancellationToken))
        {
            await SendAsync($"{_baseUrl}{Api}{year}/{period}/{collectionType}/start", cancellationToken);
        }

        public async Task ClosePeriodEndAsync(int year, int period, string collectionType, CancellationToken cancellationToken = default(CancellationToken))
        {
            await SendAsync($"{_baseUrl}{Api}{year}/{period}/{collectionType}/close", cancellationToken);
        }

        public async Task ProceedAsync(int year, int period, int path = 0, CancellationToken cancellationToken = default(CancellationToken))
        {
            await SendAsync($"{_baseUrl}{Api}{year}/{period}/{path}/proceed", cancellationToken);
        }

        public async Task<string> GetPathItemStatesAsync(int? year, int? period, string collectionType, CancellationToken cancellationToken)
        {
            var data = await GetDataAsync($"{_baseUrl}{Api}states-main/{collectionType}/{year}/{period}", cancellationToken);
            return data;
        }

        public async Task ReSubmitFailedJobAsync(long jobId, CancellationToken cancellationToken = default(CancellationToken))
        {
            var jobStatusDto = new JobStatusDto(jobId, Convert.ToInt32(JobStatusType.Ready));
            await SendDataAsync($"{_baseUrl}/api/job/{JobStatusType.Ready}", jobStatusDto, cancellationToken);
        }

        public async Task<IEnumerable<FileUploadJobMetaDataModel>> GetSubmissionsPerPeriodAsync(int year, int period, CancellationToken cancellationToken)
        {
            // get job info from db
            var files = (await GetSubmittedFilesPerPeriodAsync(year, period, cancellationToken)).ToList();

            // get file info from result report
            await Task.WhenAll(
                files.Where(f => f.JobStatus == 4)
                    .Select(file => _fileUploadJobMetaDataModelBuilderService.PopulateFileUploadJobMetaDataModel(file, period, cancellationToken)));

            return files;
        }

        public async Task<IEnumerable<FileUploadJobMetaDataModel>> GetSubmittedFilesPerPeriodAsync(int collectionYear, int period, CancellationToken cancellationToken)
        {
            var url = $"{_baseUrl}{Api}file-uploads/{collectionYear}/{period}";

            var data = await GetAsync<IEnumerable<FileUploadJobMetaDataModel>>(url, cancellationToken);

            return data;
        }

        public async Task SubmitJob(int period, string collectionName, string userName, string email, IFormFile file, CancellationToken cancellationToken)
        {
            if (file == null)
            {
                return;
            }

            var collection = await _collectionService.GetCollectionAsync(collectionName, cancellationToken);

            try
            {
                var fileName = Path.GetFileName(file.FileName);

                var job = new JobSubmission {
                    CollectionName = collection.CollectionTitle,
                    FileName = fileName,
                    FileSizeBytes = file.Length,
                    SubmittedBy = userName,
                    NotifyEmail = email,
                    StorageReference = collection.StorageReference,
                    Period = period,
                    CollectionYear = collection.CollectionYear
                };

                // add to the queue
                await _jobService.SubmitJob(job, cancellationToken);

                await (await _storageService.GetAzureStorageReferenceService(_azureStorageConfig.ConnectionString, collection.StorageReference))
                    .SaveAsync(fileName, file.OpenReadStream(), cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error trying to submit ALLF file with name : {file.Name}", ex);
                throw;
            }
        }
    }
}