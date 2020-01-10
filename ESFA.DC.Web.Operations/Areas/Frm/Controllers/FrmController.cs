using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using ESFA.DC.Jobs.Model.Enums;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Areas.Frm.Models;
using ESFA.DC.Web.Operations.Areas.Provider.Models;
using ESFA.DC.Web.Operations.Interfaces.Frm;
using ESFA.DC.Web.Operations.Interfaces.PeriodEnd;
using ESFA.DC.Web.Operations.Interfaces.Storage;
using ESFA.DC.Web.Operations.Settings.Models;
using ESFA.DC.Web.Operations.Utils;
using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.Web.Operations.Areas.Frm.Controllers
{
    [Area(AreaNames.Frm)]
    public class FrmController : Controller
    {
        private readonly IFrmService _frmService;
        private readonly ILogger _logger;
        private readonly IPeriodService _periodService;
        private readonly IStorageService _storageService;

        public FrmController(
            ILogger logger,
            IFrmService frmService,
            IPeriodService periodService,
            IStorageService storageService)
        {
            _logger = logger;
            _frmService = frmService;
            _periodService = periodService;
            _storageService = storageService;
        }

        public IActionResult Index()
        {
            var model = new FrmReportModel();
            model.IsFrmReportChoice = false;
            return View("Index", model);
        }

        public IActionResult SelectValidate()
        {
            return View("SelectValidate");
        }

        public async Task<IActionResult> HoldingPageAsync(FrmReportModel model, string frmJobType, long? jobId)
        {
            var frmStatus = (JobStatusType)await _frmService.GetFrmStatus(jobId);

            switch (frmStatus)
            {
                case JobStatusType.Failed:
                case JobStatusType.FailedRetry:
                    string errorMessage = $"The status was '{frmStatus}' for frm job '{jobId}'";
                    _logger.LogError(errorMessage);
                    TempData["Error"] = errorMessage;
                    return View("ErrorView");
                case JobStatusType.Completed:
                    if (frmJobType == "Validation")
                    {
                        return View("ValidateSuccess");
                    }

                    if (frmJobType == "Publish")
                    {
                        return View("PublishSuccess");
                    }

                    break;
                default:
                    break;
            }

            return View("HoldingPageAsync");
        }

        [HttpPost]
        public async Task<IActionResult> ValidateFrm(FrmReportModel model)
        {
            var currentPeriod = await _periodService.ReturnPeriod();
             model.FrmPeriod = $"R{currentPeriod.Period.ToString("D2")}";

            //TODO: Run Validation Job
            return View("ValidateSuccess", model); //TODO: pass in jobID
        }

        [HttpPost]
        public async Task<IActionResult> PublishFrm(FrmReportModel model)
        {
            //TODO: Run Publish job
            return RedirectToAction("HoldingPageAsync", new { frmJobType = "Publish" }); //TODO: pass in jobID
        }

        public async Task<IActionResult> ReportChoiceSelection(FrmReportModel model)
        {
            if (model.IsFrmReportChoice)
            {
                return RedirectToAction("SelectValidate");
            }

            return View("Index");
        }

        public async Task<FileResult> GetReportFile(string fileName)
        {
            try
            {
                var currentPeriod = await _periodService.ReturnPeriod();

                var containerName = Utils.Constants.FrmBlobContainerName.Replace(Utils.Constants.CollectionYearToken, currentPeriod.Year.ToString());

                var x = "9";
                var blobStream = await _storageService.GetFile(containerName, fileName, CancellationToken.None);

                return new FileStreamResult(blobStream, _storageService.GetMimeTypeFromFileName(fileName))
                {
                    FileDownloadName = string.IsNullOrEmpty(fileName) ? fileName : fileName
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