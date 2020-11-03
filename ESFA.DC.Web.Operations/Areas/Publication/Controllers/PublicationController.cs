using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Features.Indexed;
using ESFA.DC.FileService.Interface;
using ESFA.DC.Jobs.Model;
using ESFA.DC.Jobs.Model.Enums;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Areas.Publication.Models;
using ESFA.DC.Web.Operations.Constants;
using ESFA.DC.Web.Operations.Controllers;
using ESFA.DC.Web.Operations.Extensions;
using ESFA.DC.Web.Operations.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.Collections;
using ESFA.DC.Web.Operations.Interfaces.Publication;
using ESFA.DC.Web.Operations.Interfaces.Storage;
using ESFA.DC.Web.Operations.Models.Publication;
using ESFA.DC.Web.Operations.Utils;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.Web.Operations.Areas.Publication.Controllers
{
    [Area(AreaNames.Publication)]
    public class PublicationController : BaseControllerWithDevOpsOrAdvancedSupportPolicy
    {
        private readonly IReportsPublicationService _reportsPublicationService;
        private readonly ILogger _logger;
        private readonly IStorageService _storageService;
        private readonly ICollectionsService _collectionsService;

        public PublicationController(
            ILogger logger,
            IReportsPublicationService reportsPublicationService,
            IStorageService storageService,
            ICollectionsService collectionsService,
            TelemetryClient telemetryClient)
            : base(logger, telemetryClient)
        {
            _logger = logger;
            _reportsPublicationService = reportsPublicationService;
            _storageService = storageService;
            _collectionsService = collectionsService;
        }

        public IActionResult Index()
        {
            var model = new PublicationReportModel()
            {
                IsFrmReportChoice = false
            };
            return View("Index", model);
        }

        public async Task<IActionResult> SelectValidate()
        {
            var collections = await _collectionsService.GetCollectionsByType(CollectionTypeConstants.Publication);
            ViewData[ViewDataConstants.Collections] = collections.Select(x => x.CollectionTitle);
            return View("SelectValidate");
        }

        public async Task<IActionResult> HoldingPageAsync(JobDetails model)
        {
            var frmStatus = (JobStatusType)await _reportsPublicationService.GetFrmStatusAsync(model.JobId);
            string errorMessage;
            switch (frmStatus)
            {
                case JobStatusType.Failed:
                case JobStatusType.FailedRetry:
                    errorMessage = $"The job status was '{frmStatus}' for publication job with ID: '{model.JobId}'";
                    _logger.LogError(errorMessage);
                    TempData["Error"] = errorMessage;
                    return View("ErrorView");

                case JobStatusType.Waiting:
                    var details = await _reportsPublicationService.GetFileSubmittedDetailsAsync(model.JobId);
                    return View("ValidateSuccess", details);

                case JobStatusType.Completed:

                    try
                    {
                        await _reportsPublicationService.PublishSldAsync(model.JobId);
                    }
                    catch (Exception ex)
                    {
                        errorMessage = $"The publication Reports were not able to be published to SLD";
                        _logger.LogError(errorMessage, ex);
                        TempData["Error"] = errorMessage;
                        return View("ErrorView");
                    }

                    return View("PublishSuccess");
                default:
                    break;
            }

            return View("HoldingPageAsync", model);
        }

        [HttpPost]
        public async Task<IActionResult> ValidateFrmAsync(PublicationReportModel model)
        {
            var folderKey = model.PublicationDate.ToString("yyyy-MM-dd");
            var userName = User.Name();
            var jobId = await _reportsPublicationService.RunValidationAsync(model.CollectionName, folderKey, model.PeriodNumber, userName);

            var jobDetails = new JobDetails()
            {
                JobId = jobId,
                PeriodNumber = model.PeriodNumber,
            };

            return RedirectToAction("HoldingPageAsync", jobDetails);
        }

        [HttpPost]
        public async Task<IActionResult> PublishAsync(JobDetails model)
        {
            await _reportsPublicationService.RunPublishAsync(model.JobId);
            return RedirectToAction("HoldingPageAsync", model);
        }

        public async Task<IActionResult> ReportChoiceSelectionAsync(PublicationReportModel model)
        {
            if (model.IsFrmReportChoice)
            {
                return RedirectToAction("SelectValidate");
            }

            return RedirectToAction("Index", "UnPublish");
        }

        public IActionResult CancelFrm()
        {
            return View("CancelledFrm");
        }

        public async Task<FileResult> GetReportFileAsync(string fileName, string collectionName, string storageReference)
        {
            try
            {
                var blobStream = await _storageService.GetFile(storageReference, fileName, CancellationToken.None);

                return new FileStreamResult(blobStream, _storageService.GetMimeTypeFromFileName(fileName))
                {
                    FileDownloadName = $"{collectionName}_{fileName}"
                };
            }
            catch (Exception e)
            {
                _logger.LogError($"Download report failed for file name : {fileName}", e);
                throw;
            }
        }
    }
}