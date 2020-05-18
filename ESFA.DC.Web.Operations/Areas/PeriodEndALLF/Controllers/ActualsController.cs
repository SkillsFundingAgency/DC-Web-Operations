using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Jobs.Model.Enums;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Areas.PeriodEndALLF.Models;
using ESFA.DC.Web.Operations.Constants;
using ESFA.DC.Web.Operations.Controllers;
using ESFA.DC.Web.Operations.Extensions;
using ESFA.DC.Web.Operations.Interfaces.PeriodEnd;
using ESFA.DC.Web.Operations.Models.Enums;
using ESFA.DC.Web.Operations.Models.Job;
using ESFA.DC.Web.Operations.Utils;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.Web.Operations.Areas.PeriodEndALLF.Controllers
{
    [Area(AreaNames.PeriodEndALLF)]
    [Route(AreaNames.PeriodEndALLF + "/actuals")]
    public class ActualsController : BaseControllerWithOpsPolicy
    {
        private readonly IPeriodService _periodService;
        private readonly IALLFHistoryService _allfHistoryService;
        private readonly ILogger _logger;

        public ActualsController(IPeriodService periodService, IALLFHistoryService allfHistoryService, ILogger logger, TelemetryClient telemetryClient)
            : base(logger, telemetryClient)
        {
            _periodService = periodService;
            _allfHistoryService = allfHistoryService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var currentYearPeriod = await _periodService.ReturnPeriod(CollectionTypes.NCS, new CancellationToken());
            if (currentYearPeriod.Year == null)
            {
                throw new Exception($"Return period {currentYearPeriod.Period} has no year.");
            }

            var model = new ActualsViewModel
            {
                CurrentReturn = "A55",
                OpenUntil = new DateTime(2029, 1, 2, 1, 1, 1),
                History = (await _allfHistoryService.GetHistoryDetails(2020)).ToList()
            };

            return View(model);
        }

        [HttpPost]
        [RequestSizeLimit(524_288_000)]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            var fileName = Path.GetFileName(file?.FileName);
            //var collection = await _collectionService.GetCollectionAsync(ProvidersUploadCollectionName);
            //if (collection == null || !collection.IsOpen)
            //{
            //    _logger.LogWarning($"collection {ProvidersUploadCollectionName} is not open/available, but file is being uploaded");
            //    ModelState.AddModelError(ErrorMessageKeys.ErrorSummaryKey, $"collection {ProvidersUploadCollectionName} is not open/available.");
            //    return View();
            //}

            //var validationResult = await _fileNameValidationService.ValidateFileNameAsync(ProvidersUploadCollectionName, collection.FileNameRegex, fileName?.ToUpper(), file?.Length);

            //if (validationResult.ValidationResult != FileNameValidationResult.Valid)
            //{
            //    ModelState.AddModelError(ErrorMessageKeys.Submission_FileFieldKey, validationResult.FieldError);
            //    ModelState.AddModelError(ErrorMessageKeys.ErrorSummaryKey, validationResult.SummaryError);

            //    _logger.LogWarning($"User uploaded invalid file with name :{fileName}");
            //    return View();
            //}

            //await (await _storageService.GetAzureStorageReferenceService(_opsDataLoadServiceConfigSettings.ConnectionString, collection.StorageReference)).SaveAsync(fileName, file?.OpenReadStream());

            //var jobId = await _jobService.SubmitJob(new JobSubmission
            //{
            //    CollectionName = ProvidersUploadCollectionName,
            //    FileName = fileName,
            //    FileSizeBytes = file.Length,
            //    SubmittedBy = User.Name(),
            //    NotifyEmail = User.Email(),
            //    StorageReference = collection.StorageReference
            //});

            var jobId = 1;
            return RedirectToAction("InProgress", new { jobId });
        }

        public async Task<IActionResult> InProgress(long jobId)
        {
            ViewBag.AutoRefresh = true;
            return View();

            //var jobStatus = await _jobService.GetJobStatus(jobId);

            //if (jobStatus == JobStatusType.Failed || jobStatus == JobStatusType.FailedRetry)
            //{
            //    _logger.LogError($"Loading in progress page for job id : {jobId}, job is in status ; {jobStatus} - user will be sent to service error page");
            //    TempData["JobFailed"] = $"Job {jobId} has failed";
            //    return RedirectToAction("BulkUpload");
            //}

            //if (jobStatus != JobStatusType.Completed)
            //{
            //    return View();
            //}

            //return RedirectToAction("DownloadResults", new { jobId });
        }
    }
}