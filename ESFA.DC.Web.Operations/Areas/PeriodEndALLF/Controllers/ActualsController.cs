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

            // Do stuff with the upload
            var jobId = 1;
            return RedirectToAction("InProgress", new { jobId });
        }

        public async Task<IActionResult> InProgress(long jobId)
        {
            ViewBag.AutoRefresh = true;
            return View();

            // Need to check the status of the upload
        }
    }
}