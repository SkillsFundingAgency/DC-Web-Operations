using System.Collections.Generic;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Areas.PeriodEnd.Models;
using ESFA.DC.Web.Operations.Controllers;
using ESFA.DC.Web.Operations.Interfaces.PeriodEnd;
using ESFA.DC.Web.Operations.Utils;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.Web.Operations.Areas.PeriodEnd.Controllers
{
    [Area(AreaNames.PeriodEnd)]
    [Route(AreaNames.PeriodEnd + "/periodEndPreparation")]
    public class PeriodEndPrepController : BaseControllerWithOpsPolicy
    {
        private readonly IPeriodService _periodService;
        private readonly IPeriodEndService _periodEndService;
        private readonly IStateService _stateService;

        public PeriodEndPrepController(
            IPeriodService periodService,
            IPeriodEndService periodEndService,
            IStateService stateService,
            ILogger logger,
            TelemetryClient telemetryClient)
            : base(logger, telemetryClient)
        {
            _periodService = periodService;
            _periodEndService = periodEndService;
            _stateService = stateService;
        }

        [HttpGet("{collectionYear?}/{period?}")]
        public async Task<IActionResult> Index(int? collectionYear, int? period)
        {
            var currentYearPeriod = await _periodService.ReturnPeriod();
            currentYearPeriod.Year = currentYearPeriod.Year ?? 0;
            var model = new PeriodEndPrepViewModel();

            if (collectionYear != null && period != null)
            {
                model.Year = collectionYear.Value;
                model.Period = period.Value;
            }
            else
            {
                model.Year = currentYearPeriod.Year.Value;
                model.Period = currentYearPeriod.Period;
            }

            var isCurrentPeriodSelected = currentYearPeriod.Year == model.Year && currentYearPeriod.Period == model.Period;

            model.IsCurrentPeriod = isCurrentPeriodSelected;
            model.Closed = (isCurrentPeriodSelected && currentYearPeriod.PeriodClosed) || (collectionYear == currentYearPeriod.Year && period < currentYearPeriod.Period) || (collectionYear <= currentYearPeriod.Year);

            string state = await _periodEndService.GetPrepState(model.Year, model.Period);
            model.PeriodEndPrepModel = _stateService.GetPrepState(state);
            var mcaDetails = await _periodEndService.GetMcaDetails();
            model.GlaCodes = new List<string>();
            foreach (var mca in mcaDetails)
            {
                model.GlaCodes.Add(mca.Code);
            }

            return View(model);
        }

        [HttpPost("selectPeriod")]
        public IActionResult SelectPeriod(int collectionYear, int period)
        {
            return RedirectToAction("Index", new { collectionYear, period });
        }

        [HttpPost("resubmitJob")]
        public async Task<IActionResult> ReSubmitJob(int collectionYear, int period, int jobId)
        {
            await _periodEndService.ReSubmitFailedJob(jobId);

            return RedirectToAction("Index", new { collectionYear, period });
        }

        [HttpPost("startPeriodEnd")]
        public IActionResult StartPeriodEnd(int collectionYear, int period)
        {
            return RedirectToAction("Index", "PeriodEnd", new { collectionYear, period });
        }
    }
}