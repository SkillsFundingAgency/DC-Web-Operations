using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Jobs.Model;
using ESFA.DC.Jobs.Model.Enums;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.Collections;
using ESFA.DC.Web.Operations.Interfaces.PeriodEnd;
using ESFA.DC.Web.Operations.Interfaces.Storage;
using ESFA.DC.Web.Operations.Models;
using ESFA.DC.Web.Operations.Models.ALLF;
using ESFA.DC.Web.Operations.Models.Job;
using ESFA.DC.Web.Operations.Settings.Models;
using ESFA.DC.Web.Operations.Utils;
using Microsoft.AspNetCore.Http;

namespace ESFA.DC.Web.Operations.Services.PeriodEnd.ALLF
{
    public class ALLFPeriodEndService : IALLFPeriodEndService
    {
        private const string Api = "/api/period-end-allf/";
        private const string GenericActualsCollectionErrorReportName = "Generic Actuals Collection - Error Report";
        private const string ResultReportName = "Upload Result Report";
        private const string CollectionType = CollectionTypes.ALLF;

        private readonly IStorageService _storageService;
        private readonly IFileUploadJobMetaDataModelBuilderService _fileUploadJobMetaDataModelBuilderService;
        private readonly ICollectionsService _collectionService;
        private readonly IPeriodService _periodService;
        private readonly IStateService _stateService;
        private readonly ICloudStorageService _cloudStorageService;
        private readonly IJobService _jobService;
        private readonly ILogger _logger;
        private readonly AzureStorageSection _azureStorageConfig;
        private readonly IHttpClientService _httpClientService;

        private readonly string _baseUrl;

        public ALLFPeriodEndService(
            IStorageService storageService,
            IFileUploadJobMetaDataModelBuilderService fileUploadJobMetaDataModelBuilderService,
            ICollectionsService collectionService,
            IPeriodService periodService,
            IStateService stateService,
            ICloudStorageService cloudStorageService,
            IJobService jobService,
            ILogger logger,
            AzureStorageSection azureStorageConfig,
            ApiSettings apiSettings,
            IHttpClientService httpClientService)
        {
            _storageService = storageService;
            _fileUploadJobMetaDataModelBuilderService = fileUploadJobMetaDataModelBuilderService;

            _collectionService = collectionService;
            _periodService = periodService;
            _stateService = stateService;
            _cloudStorageService = cloudStorageService;
            _jobService = jobService;
            _logger = logger;
            _azureStorageConfig = azureStorageConfig;
            _httpClientService = httpClientService;
            _baseUrl = apiSettings.JobManagementApiBaseUrl;
        }

        public async Task InitialisePeriodEndAsync(int year, int period, string collectionType, CancellationToken cancellationToken)
        {
            await _httpClientService.SendAsync($"{_baseUrl}{Api}{year}/{period}/{collectionType}/initialise", cancellationToken);
        }

        public async Task StartPeriodEndAsync(int year, int period, string collectionType, CancellationToken cancellationToken = default(CancellationToken))
        {
            await _httpClientService.SendAsync($"{_baseUrl}{Api}{year}/{period}/{collectionType}/start", cancellationToken);
        }

        public async Task ClosePeriodEndAsync(int year, int period, string collectionType, CancellationToken cancellationToken = default(CancellationToken))
        {
            await _httpClientService.SendAsync($"{_baseUrl}{Api}{year}/{period}/{collectionType}/close", cancellationToken);
        }

        public async Task ProceedAsync(int year, int period, int path = 0, CancellationToken cancellationToken = default(CancellationToken))
        {
            await _httpClientService.SendAsync($"{_baseUrl}{Api}{year}/{period}/{path}/proceed", cancellationToken);
        }

        public async Task<string> GetPathItemStatesAsync(int? year, int? period, string collectionType, CancellationToken cancellationToken)
        {
            return await _httpClientService.GetDataAsync($"{_baseUrl}{Api}states-main/{collectionType}/{year}/{period}", cancellationToken);
        }

        public async Task ReSubmitFailedJobAsync(long jobId, CancellationToken cancellationToken = default(CancellationToken))
        {
            var jobStatusDto = new JobStatusDto(jobId, Convert.ToInt32(JobStatusType.Ready));
            await _httpClientService.SendDataAsync($"{_baseUrl}/api/job/{JobStatusType.Ready}", jobStatusDto, cancellationToken);
        }

        public async Task SubmitJob(int period, string collectionName, string userName, string email, IFormFile file, CancellationToken cancellationToken)
        {
            if (file == null)
            {
                return;
            }

            var collection = await _collectionService.GetCollectionAsync(collectionName, cancellationToken);

            var fileName = Path.GetFileName(file.FileName);
            await (await _storageService.GetAzureStorageReferenceService(_azureStorageConfig.ConnectionString, collection.StorageReference))
                .SaveAsync(fileName, file.OpenReadStream(), cancellationToken);

            try
            {
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
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error trying to submit ALLF file with name : {file.Name}", ex);
                throw;
            }
        }

        public async Task<PeriodEndViewModel> GetPathState(int? collectionYear, int? period, CancellationToken cancellationToken)
        {
            var currentYearPeriod = await _periodService.ReturnPeriod(CollectionType, cancellationToken);
            currentYearPeriod.Year = currentYearPeriod.Year ?? 0;

            collectionYear = collectionYear ?? currentYearPeriod.Year.Value;
            period = period ?? currentYearPeriod.Period;

            var pathItemStates = await GetPathItemStatesAsync(collectionYear, period, CollectionType, cancellationToken);
            var state = _stateService.GetMainState(pathItemStates);
            var lastItemJobsFinished = _stateService.AllJobsHaveCompleted(state);

            var files = await GetSubmissionsPerPeriodAsync(collectionYear.Value, period.Value, cancellationToken);

            var model = new PeriodEndViewModel
            {
                Period = period.Value,
                Year = collectionYear.Value,
                Paths = state.Paths,
                PeriodEndStarted = state.PeriodEndStarted,
                PeriodEndFinished = state.PeriodEndFinished,
                ClosePeriodEndEnabled = lastItemJobsFinished,
                Files = files
            };

            var isCurrentPeriodSelected = currentYearPeriod.Year == model.Year && currentYearPeriod.Period == model.Period;
            model.IsCurrentPeriod = isCurrentPeriodSelected;
            model.CollectionClosed = isCurrentPeriodSelected && currentYearPeriod.PeriodClosed;

            return model;
        }

        public async Task<IEnumerable<FileUploadJobMetaDataModel>> GetSubmissionsPerPeriodAsync(
            int year,
            int period,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            // get job info from db
            var files = (await GetSubmittedFilesPerPeriodAsync(year, period, cancellationToken))
                .OrderByDescending(f => f.SubmissionDate)
                .Take(Constants.MaxFilesToDisplay)
                .ToList();

            foreach (var foundPeriod in files.GroupBy(f => f.PeriodNumber).Select(f => f.Key))
            {
                var file = files
                    .Where(f => f.PeriodNumber == foundPeriod)
                    .OrderByDescending(f => f.SubmissionDate)
                    .First();
                file.UsedForPeriodEnd = true;
            }

            var container = _cloudStorageService.GetStorageContainer(Constants.ALLFStorageContainerName);

            // get file info from result report
            await Task.WhenAll(
                files
                    .Where(f => f.JobStatus == JobStatuses.JobStatus_Completed)
                    .Select(file => _fileUploadJobMetaDataModelBuilderService
                        .PopulateFileUploadJobMetaDataModel(
                            file,
                            GenericActualsCollectionErrorReportName,
                            ResultReportName,
                            container,
                            Constants.ALLFPeriodPrefix,
                            cancellationToken)));

            return files;
        }

        private async Task<IEnumerable<FileUploadJobMetaDataModel>> GetSubmittedFilesPerPeriodAsync(int collectionYear, int period, CancellationToken cancellationToken)
        {
            var url = $"{_baseUrl}{Api}file-uploads/{collectionYear}/{period}";

            return await _httpClientService.GetAsync<IEnumerable<FileUploadJobMetaDataModel>>(url, cancellationToken);
        }
    }
}