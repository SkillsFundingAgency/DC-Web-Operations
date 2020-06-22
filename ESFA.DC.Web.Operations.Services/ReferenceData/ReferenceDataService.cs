using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Web.Operations.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.Collections;
using ESFA.DC.Web.Operations.Interfaces.ReferenceData;
using ESFA.DC.Web.Operations.Interfaces.Storage;
using ESFA.DC.Web.Operations.Models;
using ESFA.DC.Web.Operations.Models.Job;
using ESFA.DC.Web.Operations.Models.ReferenceData;
using ESFA.DC.Web.Operations.Settings.Models;
using ESFA.DC.Web.Operations.Utils;
using Microsoft.AspNetCore.Http;

namespace ESFA.DC.Web.Operations.Services.ReferenceData
{
    public class ReferenceDataService : BaseHttpClientService, IReferenceDataService
    {
        private const string Api = "/api/reference-data-uploads/";
        private const string SummaryFileName = "Upload Result Report";
        private const string CreatedByPlaceHolder = "Data unavailable";

        private readonly ICollectionsService _collectionsService;
        private readonly IJobService _jobService;
        private readonly IStorageService _storageService;
        private readonly IFileUploadJobMetaDataModelBuilderService _fileUploadJobMetaDataModelBuilderService;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly ICloudStorageService _cloudStorageService;
        private readonly AzureStorageSection _azureStorageConfig;
        private readonly ILogger _logger;

        private readonly string _baseUrl;

        public ReferenceDataService(
            ICollectionsService collectionsService,
            IJobService jobService,
            IStorageService storageService,
            IFileUploadJobMetaDataModelBuilderService fileUploadJobMetaDataModelBuilderService,
            IJsonSerializationService jsonSerializationService,
            IDateTimeProvider dateTimeProvider,
            ICloudStorageService cloudStorageService,
            ApiSettings apiSettings,
            HttpClient httpClient,
            AzureStorageSection azureStorageConfig,
            ILogger logger)
        : base(jsonSerializationService, httpClient)
        {
            _collectionsService = collectionsService;
            _jobService = jobService;
            _storageService = storageService;
            _fileUploadJobMetaDataModelBuilderService = fileUploadJobMetaDataModelBuilderService;
            _dateTimeProvider = dateTimeProvider;
            _cloudStorageService = cloudStorageService;
            _azureStorageConfig = azureStorageConfig;
            _logger = logger;

            _baseUrl = apiSettings.JobManagementApiBaseUrl;
        }

        public async Task SubmitJob(int period, string collectionName, string userName, string email, IFormFile file, CancellationToken cancellationToken)
        {
            if (file == null)
            {
                return;
            }

            var collection = await _collectionsService.GetCollectionAsync(collectionName, cancellationToken);

            try
            {
                var fileName = $"{collectionName}/{Path.GetFileName(file.FileName)}";

                var job = new JobSubmission
                {
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
                _logger.LogError($"Error trying to submit Campus Identifiers Reference Data file with name : {file.Name}", ex);
                throw;
            }
        }

        public async Task<ReferenceDataViewModel> GetSubmissionsPerCollectionAsync(
            string containerName,
            string collectionName,
            string reportName,
            int maxRows = Constants.MaxFilesToDisplay,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var model = new ReferenceDataViewModel();

            // get job info from db
            var files = (await GetSubmittedFilesPerCollectionAsync(collectionName, cancellationToken))
                .OrderByDescending(f => f.SubmissionDate)
                .Take(maxRows)
                .ToList();

            var container = _cloudStorageService.GetStorageContainer(containerName);

            // get file info from result report
            await Task.WhenAll(
                files
                    .Select(file => _fileUploadJobMetaDataModelBuilderService
                        .PopulateFileUploadJobMetaDataModelForReferenceData(
                            file,
                            reportName,
                            SummaryFileName,
                            container,
                            collectionName,
                            cancellationToken)));

            model.Files = files;
            return model;
        }

        public async Task<ReferenceDataIndexModel> GetLatestReferenceDataJobs(CancellationToken cancellationToken)
        {
            var jobs = await _jobService.GetLatestJobForReferenceDataCollectionsAsync(CollectionTypes.ReferenceData, cancellationToken);

            var latestSuccessfulCIJob = jobs.FirstOrDefault(j => j.CollectionName == CollectionNames.ReferenceDataCampusIdentifiers);
            var latestSuccessfulCoFRJob = jobs.FirstOrDefault(j => j.CollectionName == CollectionNames.ReferenceDataConditionsOfFundingRemoval);
            var latestSuccessfulVal2021Job = jobs.FirstOrDefault(j => j.CollectionName == CollectionNames.ReferenceDataValidationMessages2021);

            var model = new ReferenceDataIndexModel
            {
                CampusIdentifiers = new ReferenceDataIndexBase
                {
                    LastUpdatedDateTime = latestSuccessfulCIJob.DateTimeSubmittedUtc != null ? _dateTimeProvider.ConvertUtcToUk(latestSuccessfulCIJob.DateTimeSubmittedUtc) : DateTime.MinValue,
                    LastUpdatedByWho = latestSuccessfulCIJob?.CreatedBy ?? CreatedByPlaceHolder
                },
                ConditionOfFundingRemoval = new ReferenceDataIndexBase
                {
                    LastUpdatedDateTime = latestSuccessfulCoFRJob?.DateTimeSubmittedUtc != null ? _dateTimeProvider.ConvertUtcToUk(latestSuccessfulCoFRJob.DateTimeSubmittedUtc) : DateTime.MinValue,
                    LastUpdatedByWho = latestSuccessfulCoFRJob?.CreatedBy ?? CreatedByPlaceHolder
                },
                ValidationMessages2021 = new ReferenceDataIndexBase
                {
                    LastUpdatedDateTime = latestSuccessfulVal2021Job?.DateTimeSubmittedUtc != null ? _dateTimeProvider.ConvertUtcToUk(latestSuccessfulVal2021Job.DateTimeSubmittedUtc) : DateTime.MinValue,
                    LastUpdatedByWho = latestSuccessfulVal2021Job?.CreatedBy ?? CreatedByPlaceHolder
                }
            };

            return model;
        }

        private async Task<IEnumerable<FileUploadJobMetaDataModel>> GetSubmittedFilesPerCollectionAsync(string collectionName, CancellationToken cancellationToken)
        {
            var url = $"{_baseUrl}{Api}file-uploads/{collectionName}";

            var data = await GetAsync<IEnumerable<FileUploadJobMetaDataModel>>(url, cancellationToken);

            return data;
        }
    }
}
