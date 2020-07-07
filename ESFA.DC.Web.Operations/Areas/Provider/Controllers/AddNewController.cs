using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Features.Indexed;
using ESFA.DC.Jobs.Model;
using ESFA.DC.Jobs.Model.Enums;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Web.Operations.Areas.Provider.Models;
using ESFA.DC.Web.Operations.Constants;
using ESFA.DC.Web.Operations.Controllers;
using ESFA.DC.Web.Operations.Extensions;
using ESFA.DC.Web.Operations.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.Collections;
using ESFA.DC.Web.Operations.Interfaces.Provider;
using ESFA.DC.Web.Operations.Interfaces.ReferenceData;
using ESFA.DC.Web.Operations.Interfaces.Storage;
using ESFA.DC.Web.Operations.Models.Enums;
using ESFA.DC.Web.Operations.Models.Job;
using ESFA.DC.Web.Operations.Settings.Models;
using ESFA.DC.Web.Operations.Utils;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.Web.Operations.Areas.Provider.Controllers
{
    [Area(AreaNames.Provider)]
    public class AddNewController : BaseControllerWithDevOpsOrAdvancedSupportPolicy
    {
        private const string TemplatesPath = @"\\templates";
        private const string BulkUploadFileName = @"MultipleProvidersTemplate.csv";
        private const string ProvidersUploadCollectionName = @"REF-OPS";
        private readonly string SummaryFileName = "{0}/Summary.json";
        private readonly string ErrorsFileName = "{0}/Errors.csv";
        private readonly ILogger _logger;
        private readonly ICollectionsService _collectionService;
        private readonly IStorageService _storageService;
        private readonly OpsDataLoadServiceConfigSettings _opsDataLoadServiceConfigSettings;
        private readonly IJobService _jobService;
        private readonly IJsonSerializationService _jsonSerializationService;
        private readonly IAddNewProviderService _addNewProviderService;
        private readonly IFileNameValidationServiceProvider _fileNameValidationServiceProvider;

        public AddNewController(
            ILogger logger,
            IAddNewProviderService addNewProviderService,
            ICollectionsService collectionService,
            IStorageService storageService,
            OpsDataLoadServiceConfigSettings opsDataLoadServiceConfigSettings,
            IJobService jobService,
            IFileNameValidationServiceProvider fileNameValidationServicesProvider,
            IJsonSerializationService jsonSerializationService,
            TelemetryClient telemetryClient)
            : base(logger, telemetryClient)
        {
            _logger = logger;
            _collectionService = collectionService;
            _storageService = storageService;
            _opsDataLoadServiceConfigSettings = opsDataLoadServiceConfigSettings;
            _jobService = jobService;
            _jsonSerializationService = jsonSerializationService;
            _addNewProviderService = addNewProviderService;
            _fileNameValidationServiceProvider = fileNameValidationServicesProvider;
        }

        [HttpGet]
        public async Task<IActionResult> Index(long? ukprn = null)
        {
            var model = new ProviderViewModel();

            if (ukprn.HasValue)
            {
                var provider = await _addNewProviderService.GetProvider(ukprn.Value);
                model.Ukprn = provider.Ukprn;
                model.ProviderName = provider.Name;
                model.Upin = provider.Upin;
                model.IsMca = provider.IsMca.GetValueOrDefault();
            }

            return View("Index", model);
        }

        [HttpGet]
        public async Task<IActionResult> AddNewOption()
        {
            var model = new ProviderViewModel
            {
                IsSingleAddNewProviderChoice = true
            };

            return View(model);
        }

        public async Task<IActionResult> BulkUpload()
        {
            if (TempData["JobFailed"] != null)
            {
                ModelState.AddModelError(ErrorMessageKeys.ErrorSummaryKey, TempData["JobFailed"].ToString());
            }

            return View("BulkUpload");
        }

        [HttpPost]
        [RequestSizeLimit(524_288_000)]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> BulkUpload(IFormFile file, CancellationToken cancellationToken)
        {
            if (file == null)
            {
                return View("BulkUpload");
            }

            var fileName = Path.GetFileName(file.FileName);
            var collection = await _collectionService.GetCollectionAsync(ProvidersUploadCollectionName);
            if (collection == null || !collection.IsOpen)
            {
                _logger.LogWarning($"collection {ProvidersUploadCollectionName} is not open/available, but file is being uploaded");
                ModelState.AddModelError(ErrorMessageKeys.ErrorSummaryKey, $"collection {ProvidersUploadCollectionName} is not open/available.");
                return View();
            }

            var fileNameValidationService = _fileNameValidationServiceProvider.GetFileNameValidationService(CollectionNames.ReferenceDataOps);

            var validationResult = await fileNameValidationService.ValidateFileNameAsync(fileName.ToUpper(CultureInfo.CurrentUICulture), file.Length, cancellationToken);

            if (validationResult.ValidationResult != FileNameValidationResult.Valid)
            {
                ModelState.AddModelError(ErrorMessageKeys.Submission_FileFieldKey, validationResult.FieldError);
                ModelState.AddModelError(ErrorMessageKeys.ErrorSummaryKey, validationResult.SummaryError);

                _logger.LogWarning($"User uploaded invalid file with name :{fileName}");
                return View();
            }

            await (await _storageService.GetAzureStorageReferenceService(_opsDataLoadServiceConfigSettings.ConnectionString, collection.StorageReference)).SaveAsync(fileName, file?.OpenReadStream());

            var jobId = await _jobService.SubmitJob(
                new JobSubmission
            {
                CollectionName = ProvidersUploadCollectionName,
                FileName = fileName,
                FileSizeBytes = file.Length,
                SubmittedBy = User.Name(),
                NotifyEmail = User.Email(),
                StorageReference = collection.StorageReference
            }, cancellationToken);

            return RedirectToAction("InProgress", new { jobId });
        }

        public async Task<IActionResult> InProgress(long jobId)
        {
            ViewBag.AutoRefresh = true;

            var jobStatus = await _jobService.GetJobStatus(jobId);

            if (jobStatus == JobStatusType.Failed || jobStatus == JobStatusType.FailedRetry)
            {
                _logger.LogError($"Loading in progress page for job id : {jobId}, job is in status ; {jobStatus} - user will be sent to service error page");
                TempData["JobFailed"] = $"Job {jobId} has failed";
                return RedirectToAction("BulkUpload");
            }

            if (jobStatus != JobStatusType.Completed)
            {
                return View();
            }

            return RedirectToAction("DownloadResults", new { jobId });
        }

        public async Task<IActionResult> DownloadResults(long jobId)
        {
            var model = new DownloadResultsViewModel();
            var summaryFileName = string.Format(SummaryFileName, jobId);
            var errorsFileName = string.Format(ErrorsFileName, jobId);
            var job = await _jobService.GetJob(0, jobId);
            model.JobId = job.JobId;
            model.ContainerName = job.StorageReference;

            // json file
            var persistenceService = await _storageService.GetAzureStorageReferenceService(_opsDataLoadServiceConfigSettings.ConnectionString, job.StorageReference);
            var summaryExists = await persistenceService.ContainsAsync(summaryFileName);
            if (summaryExists)
            {
                var data = await persistenceService.GetAsync(summaryFileName);
                var providersUploadSummary = _jsonSerializationService.Deserialize<ProvidersUploadSummary>(data);
                model.TotalSuccessful = providersUploadSummary.NumberOfSuccess;
                model.TotalFailed = providersUploadSummary.NumberOfFail;
            }

            // errors file
            var errorsFileExists = await persistenceService.ContainsAsync(errorsFileName);
            if (errorsFileExists)
            {
                model.ErrorFileExists = true;
            }

            model.ErrorsFileName = errorsFileName;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DownloadResultsFile(string containerName, string errorsFileName, int jobId)
        {
            var blobStream = await _storageService.GetFile(containerName, errorsFileName, CancellationToken.None);

            return new FileStreamResult(blobStream, _storageService.GetMimeTypeFromFileName(errorsFileName))
            {
                FileDownloadName = $"Errors_{jobId}.csv"
            };
        }

        public FileResult DownloadTemplate()
        {
            var assembly = Assembly.GetExecutingAssembly();
            string resourceName = assembly.GetManifestResourceNames().Single(str => str.EndsWith(BulkUploadFileName));
            var manifestResourceStream = assembly.GetManifestResourceStream(resourceName);
            var mimeType = "application/csv";

            return File(manifestResourceStream, mimeType, BulkUploadFileName);
        }

        [HttpPost]
        public async Task<IActionResult> AddNewChoiceSubmit(ProviderViewModel model)
        {
            if (model.IsSingleAddNewProviderChoice)
            {
                return RedirectToAction("Index");
            }

            return RedirectToAction("BulkUpload");
        }

        [HttpPost]
        public async Task<IActionResult> AddSingleProvider(ProviderViewModel model)
        {
            _logger.LogDebug("Entered AddSingleProvider");

            const string ErrorSavingOrganisation = "An error occured saving the organisation.";

            if (!ModelState.IsValid)
            {
                return View("Index", model);
            }

            if (!(await _addNewProviderService.AddProvider(
                new Operations.Models.Provider.Provider(model.ProviderName, model.Ukprn.Value, model.Upin, model.IsMca),
                CancellationToken.None)))
            {
                ModelState.AddModelError("Summary", ErrorSavingOrganisation);
                return View("Index", model);
            }

            _logger.LogDebug("Exit AddSingleProvider");
            return RedirectToAction("Index", "ManageProviders", new { ukprn = model.Ukprn });
        }
    }
}