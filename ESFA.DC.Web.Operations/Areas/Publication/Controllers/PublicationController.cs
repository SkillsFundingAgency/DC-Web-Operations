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
using ESFA.DC.Web.Operations.Interfaces.Frm;
using ESFA.DC.Web.Operations.Interfaces.Storage;
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

        public async Task<IActionResult> HoldingPageAsync(PublicationReportModel model)
        {
            var frmStatus = (JobStatusType)await _reportsPublicationService.GetFrmStatusAsync(model.FrmJobId);
            string errorMessage;
            switch (frmStatus)
            {
                case JobStatusType.Failed:
                case JobStatusType.FailedRetry:
                    errorMessage = $"The job status was '{frmStatus}' for FRM job with ID: '{model.FrmJobId}'";
                    _logger.LogError(errorMessage);
                    TempData["Error"] = errorMessage;
                    return View("ErrorView");
                case JobStatusType.Waiting:
                    var details = await _reportsPublicationService.GetFileSubmittedDetailsAsync(model.FrmJobId);
                    return View("ValidateSuccess", details);
                case JobStatusType.Completed:

                    try
                    {
                        await _reportsPublicationService.PublishSldAsync(model.PublicationYearPeriod, model.PeriodNumber);
                    }
                    catch (Exception ex)
                    {
                        errorMessage = $"The FRM Reports were not able to be published to SLD";
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
            model.FrmJobType = Utils.Constants.PublicationValidationJobKey;
            var folderKey = model.PublicationDate.ToString("yyyy-MM-dd");
            var userName = User.Name();
            model.FrmJobId = await _reportsPublicationService.RunValidationAsync(model.CollectionName, folderKey, model.PeriodNumber, userName);

            return RedirectToAction("HoldingPageAsync", model);
        }

        [HttpPost]
        public async Task<IActionResult> PublishFrmAsync(PublicationReportModel model)
        {
            model.FrmJobType = Utils.Constants.FrmPublishKey;
            model.FrmJobId = await _reportsPublicationService.RunPublishAsync(model.FrmJobId);
            return RedirectToAction("HoldingPageAsync", model);
        }

        public async Task<IActionResult> ReportChoiceSelectionAsync(PublicationReportModel model)
        {
            if (model.IsFrmReportChoice)
            {
                return RedirectToAction("SelectValidate");
            }

            var collectionType = "frm";
            var reportsData = await _reportsPublicationService.GetFrmReportsDataAsync();
            var lastTwoYears = await _reportsPublicationService.GetLastTwoCollectionYearsAsync(collectionType);
            var lastYearValue = lastTwoYears.LastOrDefault();
            model.PublishedFrm = reportsData.Where(x => x.CollectionYear == lastYearValue); // get all the open periods from the latest year period

            if (lastTwoYears.Count() > 1) //if there are more than two years in the collection
            {
                var firstYearValue = lastTwoYears.First();
                var firstYearList = reportsData.Where(x => x.CollectionYear == firstYearValue).TakeLast(1); //take the most recent open period in the previous year
                model.PublishedFrm = firstYearList.Concat(model.PublishedFrm); // add it to the front of the list
            }

            return View("SelectUnpublish", model);
        }

        public IActionResult CancelFrm()
        {
            return View("CancelledFrm");
        }

        public async Task<IActionResult> UnpublishFrmAsync(string path)
        {
            try
            {
                var pathArray = path.Split("/");
                int yearPeriod = int.Parse(pathArray[0]);
                int periodNumber = int.Parse(pathArray[1]);
                await _reportsPublicationService.UnpublishSldAsync(periodNumber, yearPeriod);
                string containerName = $"frm{yearPeriod}-files";
                await _reportsPublicationService.UnpublishSldDeleteFolderAsync(containerName, periodNumber);
                return View("UnpublishSuccess");
            }
            catch (Exception ex)
            {
                string errorMessage = $"The FRM Reports were not able to be unpublished from SLD";
                _logger.LogError(errorMessage, ex);
                TempData["Error"] = errorMessage;
                return View("ErrorView");
            }
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