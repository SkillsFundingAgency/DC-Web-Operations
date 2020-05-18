using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Features.AttributeFilters;
using Autofac.Features.Indexed;
using ESFA.DC.FileService.Interface;
using ESFA.DC.Jobs.Model.Enums;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Areas.Frm.Models;
using ESFA.DC.Web.Operations.Controllers;
using ESFA.DC.Web.Operations.Extensions;
using ESFA.DC.Web.Operations.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.Frm;
using ESFA.DC.Web.Operations.Interfaces.Storage;
using ESFA.DC.Web.Operations.Utils;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.Web.Operations.Areas.Frm.Controllers
{
    [Area(AreaNames.Frm)]
    public class FrmController : BaseControllerWithOpsPolicy
    {
        private readonly IFrmService _frmService;
        private readonly ILogger _logger;
        private readonly IStorageService _storageService;
        private readonly IFileService _fileService;

        public FrmController(
            ILogger logger,
            IFrmService frmService,
            IStorageService storageService,
            IIndex<PersistenceStorageKeys, IFileService> fileService,
            TelemetryClient telemetryClient)
            : base(logger, telemetryClient)
        {
            _logger = logger;
            _frmService = frmService;
            _storageService = storageService;
            _fileService = fileService[PersistenceStorageKeys.DctAzureStorage];
        }

        public IActionResult Index()
        {
            var model = new FrmReportModel()
            {
                IsFrmReportChoice = false
            };
            return View("Index", model);
        }

        public IActionResult SelectValidate()
        {
            return View("SelectValidate");
        }

        public async Task<IActionResult> HoldingPageAsync(FrmReportModel model)
        {
            var frmStatus = (JobStatusType)await _frmService.GetFrmStatusAsync(model.FrmJobId);
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
                    var publishPeriod = model.FrmPeriodNumber;
                    model.FrmPeriod = $"R{publishPeriod.ToString("D2")}";
                    var currentContainerName = string.Format(Utils.Constants.FrmContainerName, model.FrmYearPeriod);
                    model.FrmCSVValidDate = await _frmService.GetFileSubmittedDateAsync(model.FrmJobId);
                    return View("ValidateSuccess", model);
                case JobStatusType.Completed:

                    try
                    {
                        await _frmService.PublishSldAsync(model.FrmYearPeriod, model.FrmPeriodNumber);
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
        public async Task<IActionResult> ValidateFrmAsync(FrmReportModel model)
        {
            model.FrmJobType = Utils.Constants.FrmValidationKey;
            var frmContainerName = $"frm{model.FrmYearPeriod}";
            var frmFolderKey = model.FrmDate.ToString("yyyy-MM-dd");
            var collectionYear = model.FrmYearPeriod;
            var userName = User.Name();
            var currentContainerName = string.Format(Utils.Constants.FrmContainerName, collectionYear);
            model.FrmJobId = await _frmService.RunValidationAsync(frmContainerName, frmFolderKey, model.FrmPeriodNumber, currentContainerName, userName);

            return RedirectToAction("HoldingPageAsync", model);
        }

        [HttpPost]
        public async Task<IActionResult> PublishFrmAsync(FrmReportModel model)
        {
            model.FrmJobType = Utils.Constants.FrmPublishKey;
            model.FrmJobId = await _frmService.RunPublishAsync(model.FrmJobId);
            return RedirectToAction("HoldingPageAsync", model);
        }

        public async Task<IActionResult> ReportChoiceSelectionAsync(FrmReportModel model)
        {
            if (model.IsFrmReportChoice)
            {
                return RedirectToAction("SelectValidate");
            }

            var collectionType = "frm";
            var reportsData = await _frmService.GetFrmReportsDataAsync();
            var lastTwoYears = await _frmService.GetLastTwoCollectionYearsAsync(collectionType);
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
                await _frmService.UnpublishSldAsync(periodNumber, yearPeriod);
                string containerName = $"frm{yearPeriod}-files";
                await _frmService.UnpublishSldDeleteFolderAsync(containerName, periodNumber);
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

        public async Task<FileResult> GetReportFileAsync(string fileName, int yearPeriod)
        {
            try
            {
                var containerName = string.Format(Utils.Constants.FrmContainerName, yearPeriod);

                var blobStream = await _storageService.GetFile(containerName, fileName, CancellationToken.None);

                return new FileStreamResult(blobStream, _storageService.GetMimeTypeFromFileName(fileName))
                {
                    FileDownloadName = fileName
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