using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.FileService.Interface;
using ESFA.DC.Jobs.Model.Enums;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Areas.Frm.Models;
using ESFA.DC.Web.Operations.Controllers;
using ESFA.DC.Web.Operations.Extensions;
using ESFA.DC.Web.Operations.Interfaces.Frm;
using ESFA.DC.Web.Operations.Interfaces.PeriodEnd;
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
        private readonly IPeriodService _periodService;
        private readonly IStorageService _storageService;
        private readonly IFileService _fileService;

        public FrmController(
            ILogger logger,
            IFrmService frmService,
            IPeriodService periodService,
            IStorageService storageService,
            IFileService fileService,
            TelemetryClient telemetryClient)
            : base(logger, telemetryClient)
        {
            _logger = logger;
            _frmService = frmService;
            _periodService = periodService;
            _storageService = storageService;
            _fileService = fileService;
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

        public async Task<IActionResult> HoldingPageAsync(FrmReportModel model, string frmJobType)
        {
            var frmStatus = (JobStatusType)await _frmService.GetFrmStatus(model.FrmJobId);

            switch (frmStatus)
            {
                case JobStatusType.Failed:
                case JobStatusType.FailedRetry:
                    string errorMessage = $"The status was '{frmStatus}' for frm job '{model.FrmJobId}'";
                    _logger.LogError(errorMessage);
                    TempData["Error"] = errorMessage;
                    return View("ErrorView");
                case JobStatusType.Waiting:
                    var currentPeriod = await _periodService.ReturnPeriod();
                    model.FrmPeriod = $"R{currentPeriod.Period.ToString("D2")}";
                    model.FrmYearPeriod = currentPeriod.Year.Value;
                    var currentContainerName = string.Format(Utils.Constants.FrmContainerName, model.FrmYearPeriod);
                    model.FrmCSVValidDate = await _frmService.GetFileSubmittedDate(model.FrmJobId);
                    return View("ValidateSuccess", model);
                case JobStatusType.Completed:
                    await _frmService.PublishSld(model.FrmYearPeriod, model.FrmPeriodNumber);
                    return View("PublishSuccess");
                default:
                    break;
            }

            return View("HoldingPageAsync", model);
        }

        [HttpPost]
        public async Task<IActionResult> ValidateFrm(FrmReportModel model)
        {
            var currentYearPeriod = await _periodService.ReturnPeriod();
            if (currentYearPeriod?.Year == null)
            {
                string errorMessage = $"Call to get current return period failed";
                _logger.LogError(errorMessage);
                throw new Exception(errorMessage);
            }

            model.FrmJobType = Utils.Constants.FrmValidationKey;
            var frmContainerName = $"frm{model.FrmYearPeriod}";
            model.FrmPeriodNumber = currentYearPeriod.Period;
            var frmFolderKey = model.FrmDate.ToString("yyyy-MM-dd");
            var collectionYear = currentYearPeriod.Year.Value;
            var userName = User.Name();
            var currentContainerName = string.Format(Utils.Constants.FrmContainerName, collectionYear);
            model.FrmJobId = await _frmService.RunValidation(frmContainerName, frmFolderKey, model.FrmPeriodNumber, currentContainerName, userName);

            return RedirectToAction("HoldingPageAsync", model);
        }

        [HttpPost]
        public async Task<IActionResult> PublishFrm(FrmReportModel model)
        {
            model.FrmJobType = Utils.Constants.FrmPublishKey;
            model.FrmJobId = await _frmService.RunPublish(model.FrmJobId);
            return RedirectToAction("HoldingPageAsync", model);
        }

        public async Task<IActionResult> ReportChoiceSelection(FrmReportModel model)
        {
            if (model.IsFrmReportChoice)
            {
                return RedirectToAction("SelectValidate");
            }

            var collectionType = "frm";
            var reportsData = await _frmService.GetFrmReportsData();
            var lastTwoYears = await _frmService.GetLastTwoCollectionYears(collectionType);
            var lastYearValue = lastTwoYears.Last();
            model.PublishedFrm = reportsData.Where(x => x.CollectionYear == lastYearValue); // get all the open periods from the latest year period

            if (lastTwoYears.Count() > 1) //if there are more than two years in the collection
            {
                var firstYearValue = lastTwoYears.First();
                var firstYearList = reportsData.Where(x => x.CollectionYear == firstYearValue).TakeLast(1); //take the most recent open period in the previous year
                model.PublishedFrm = firstYearList.Concat(model.PublishedFrm); // add it to the front of the list
            }

            if (!model.PublishedFrm.Any())
            {
                return View("ErrorView");
            }

            return View("SelectUnpublish", model);
        }

        public IActionResult CancelFrm()
        {
            return View("CancelledFrm");
        }

        public async Task<IActionResult> UnpublishFrmAsync(string path)
        {
            await _frmService.UnpublishSld(path);
            return View("UnpublishSuccess");
        }

        public async Task<FileResult> GetReportFile(string fileName)
        {
            try
            {
                var currentPeriod = await _periodService.ReturnPeriod();

                var containerName = string.Format(Utils.Constants.FrmContainerName, currentPeriod.Year);

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