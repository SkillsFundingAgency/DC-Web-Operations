using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Jobs.Model.Enums;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Areas.Frm.Models;
using ESFA.DC.Web.Operations.Areas.Provider.Models;
using ESFA.DC.Web.Operations.Interfaces.Frm;
using ESFA.DC.Web.Operations.Interfaces.PeriodEnd;
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

        public FrmController(
            ILogger logger,
            IFrmService frmService,
            IPeriodService periodService)
        {
            _logger = logger;
            _frmService = frmService;
            _periodService = periodService;
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
                        var currentPeriod = await _periodService.ReturnPeriod();
                        model.FrmPeriod = currentPeriod.Period;
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
            var frmDateString = model.FrmDate.ToString("yyyy-dd-MM");
            var frmPeriodString = $"frm{model.FrmYearPeriod}";
            //TODO: Run Validation Job
            return View("ValidateSuccess" /*new { frmJobType = "Validation" }*/); //TODO: pass in jobID
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
    }
}