using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Features.Indexed;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.FileService.Interface;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.Collections;
using ESFA.DC.Web.Operations.Interfaces.ReferenceData;
using ESFA.DC.Web.Operations.Models.Job;
using ESFA.DC.Web.Operations.Models.ReferenceData;
using ESFA.DC.Web.Operations.Utils;
using Microsoft.AspNetCore.Http;
using MoreLinq;

namespace ESFA.DC.Web.Operations.Services.ReferenceData
{
    public class ReferenceDataService : IReferenceDataService
    {
        private const string Api = "/api/reference-data-uploads/";
        private const string SummaryFileName = "Upload Result Report";
        private const string DataUnavailable = "Data unavailable";

        private readonly ICollectionsService _collectionsService;
        private readonly IJobService _jobService;
        private readonly IFileUploadJobMetaDataModelBuilderService _fileUploadJobMetaDataModelBuilderService;
        private readonly IFundingClaimsDatesService _fundingClaimsDatesService;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly ICloudStorageService _cloudStorageService;
        private readonly ILogger _logger;
        private readonly IReferenceDataServiceClient _referenceDataServiceClient;
        private readonly IFileService _fileService;
        private readonly IEnumerable<ICollection> _collections;

        public ReferenceDataService(
            ICollectionsService collectionsService,
            IJobService jobService,
            IFileUploadJobMetaDataModelBuilderService fileUploadJobMetaDataModelBuilderService,
            IFundingClaimsDatesService fundingClaimsDatesService,
            IDateTimeProvider dateTimeProvider,
            ICloudStorageService cloudStorageService,
            ILogger logger,
            IReferenceDataServiceClient referenceDataServiceClient,
            IIndex<PersistenceStorageKeys, IFileService> operationsFileService,
            IEnumerable<ICollection> collections)
        {
            _collectionsService = collectionsService;
            _jobService = jobService;
            _fileUploadJobMetaDataModelBuilderService = fileUploadJobMetaDataModelBuilderService;
            _fundingClaimsDatesService = fundingClaimsDatesService;
            _dateTimeProvider = dateTimeProvider;
            _cloudStorageService = cloudStorageService;
            _logger = logger;
            _referenceDataServiceClient = referenceDataServiceClient;
            _collections = collections;
            _fileService = operationsFileService[PersistenceStorageKeys.DctAzureStorage];
        }

        public async Task SubmitJobAsync(int period, string collectionName, string userName, string email, IFormFile file, CancellationToken cancellationToken)
        {
            if (file == null)
            {
                return;
            }

            var fileName = $"{collectionName}/{Path.GetFileName(file.FileName)}";
            await SubmitJobAndPutFileInStorage(period, collectionName, userName, email, file, fileName, cancellationToken);
        }

        public async Task SubmitJobAsync(int period, string collectionName, string userName, string email, IFormFile file, string containingFolder, CancellationToken cancellationToken)
        {
            if (file == null
                || string.IsNullOrWhiteSpace(containingFolder))
            {
                return;
            }

            var fileName = $"{containingFolder}/{collectionName}/{Path.GetFileName(file.FileName)}";
            await SubmitJobAndPutFileInStorage(period, collectionName, userName, email, file, fileName, cancellationToken);
        }

        public async Task SubmitJobAsync(int period, string collectionName, string userName, string email, CancellationToken cancellationToken)
        {
            var job = await CreateJobSubmissionAsync(period, collectionName, userName, email, "ValueNeededButNotUsed", 0, cancellationToken);

            // add to the queue
            await _jobService.SubmitJob(job, cancellationToken);
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
            var files = (await _referenceDataServiceClient.GetSubmittedFilesPerCollectionAsync(Api, collectionName, cancellationToken))
                .OrderByDescending(f => f.SubmissionDate)
                .Take(maxRows)
                .ToList();

            var container = _cloudStorageService.GetStorageContainer(containerName);

            // get file info from result report
            await Task.WhenAll(
                files
                    .Select(file => _fileUploadJobMetaDataModelBuilderService
                        .PopulateFileUploadJobMetaDataModelForReferenceDataUpload(
                            file,
                            reportName,
                            SummaryFileName,
                            container,
                            collectionName,
                            cancellationToken)));

            model.Files = files;
            model.ReferenceDataCollectionName = collectionName;
            return model;
        }

        public async Task<ReferenceDataIndexModel> GetLatestReferenceDataJobs(CancellationToken cancellationToken)
        {
            var collectionsToExcludeForSummary = new string[] { CollectionNames.DevolvedPostcodesFullName, CollectionNames.DevolvedPostcodesOnsOverride, CollectionNames.DevolvedPostcodesLocalAuthority, CollectionNames.DevolvedPostcodesSof };
            var jobs = (await _jobService.GetLatestJobForReferenceDataCollectionsAsync(CollectionTypes.ReferenceData, cancellationToken))?.ToList();

            var model = new ReferenceDataIndexModel();

            foreach (var collection in _collections.Where(w => !collectionsToExcludeForSummary.Contains(w.CollectionName)))
            {
                var collectionJobs = jobs?.FirstOrDefault(j => j.CollectionName == collection.CollectionName);
                model.CollectionJobStats.Add(collection.CollectionName, new ReferenceDataIndexBase { LastUpdatedDateTime = GetDate(collectionJobs?.DateTimeSubmittedUtc), LastUpdatedByWho = collectionJobs?.CreatedBy ?? DataUnavailable, Valid = await IsCollectionValid(collection.CollectionName, cancellationToken) });
            }

            // Special Cases
            var fundingClaimsCollectionMetaDataLastUpdate = await _fundingClaimsDatesService.GetLastUpdatedFundingClaimsCollectionMetaDataAsync(cancellationToken);
            model.CollectionJobStats.Add(CollectionNames.FundingClaimsMetaData, new ReferenceDataIndexBase { LastUpdatedDateTime = GetDate(fundingClaimsCollectionMetaDataLastUpdate?.DateTimeUpdatedUtc), LastUpdatedByWho = fundingClaimsCollectionMetaDataLastUpdate?.CreatedBy ?? DataUnavailable, Valid = true });

            var latestSuccessfulDevolvedPostcodeJob = jobs?.Where(
                w => w.CollectionName == CollectionNames.DevolvedPostcodesFullName ||
                     w.CollectionName == CollectionNames.DevolvedPostcodesLocalAuthority ||
                     w.CollectionName == CollectionNames.DevolvedPostcodesOnsOverride ||
                     w.CollectionName == CollectionNames.DevolvedPostcodesSof)
                .OrderByDescending(o => o.DateTimeSubmittedUtc)
                .FirstOrDefault();

            model.CollectionJobStats.Add(CollectionNames.DevolvedPostcodes, new ReferenceDataIndexBase { LastUpdatedDateTime = GetDate(latestSuccessfulDevolvedPostcodeJob?.DateTimeSubmittedUtc), LastUpdatedByWho = latestSuccessfulDevolvedPostcodeJob?.CreatedBy ?? DataUnavailable, Valid = true });

            return model;
        }

        private async Task<bool> IsCollectionValid(string collectionName, CancellationToken cancellationToken)
        {
            return collectionName != CollectionNames.ReferenceDataValidationMessages2021 ||
                   !(await _referenceDataServiceClient.IsReferenceDataCollectionExpired(
                       CollectionNames.ReferenceDataValidationMessages2021, cancellationToken));
        }

        private async Task<JobSubmission> CreateJobSubmissionAsync(int period, string collectionName, string userName, string email, string fileName, long fileSizeInBytes, CancellationToken cancellationToken)
        {
            var collection = await _collectionsService.GetCollectionAsync(collectionName, cancellationToken);

            return new JobSubmission
            {
                CollectionName = collection.CollectionTitle,
                FileName = fileName,
                FileSizeBytes = fileSizeInBytes,
                SubmittedBy = userName,
                NotifyEmail = email,
                StorageReference = collection.StorageReference,
                Period = period,
                CollectionYear = collection.CollectionYear
            };
        }

        private async Task SubmitJobAndPutFileInStorage(int period, string collectionName, string userName, string email, IFormFile file, string fileName, CancellationToken cancellationToken)
        {
            var job = await CreateJobSubmissionAsync(period, collectionName, userName, email, fileName, file.Length, cancellationToken);

            await _jobService.SubmitJob(job, cancellationToken);

            using (Stream stream = await _fileService.OpenWriteStreamAsync(fileName, job.StorageReference, cancellationToken))
            {
                await file.CopyToAsync(stream);
            }
        }

        private DateTime GetDate(DateTime? date)
        {
            return date != null ? _dateTimeProvider.ConvertUtcToUk(date.Value) : DateTime.MinValue;
        }
    }
}
