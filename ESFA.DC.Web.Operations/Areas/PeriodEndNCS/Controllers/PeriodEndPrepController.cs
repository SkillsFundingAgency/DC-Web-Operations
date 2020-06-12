using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Areas.PeriodEndNCS.Models;
using ESFA.DC.Web.Operations.Controllers;
using ESFA.DC.Web.Operations.Interfaces.PeriodEnd;
using ESFA.DC.Web.Operations.Utils;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.Web.Operations.Areas.PeriodEndNCS.Controllers
{
    [Area(AreaNames.PeriodEndNCS)]
    [Route(AreaNames.PeriodEndNCS + "/periodEndPreparation")]
    public class PeriodEndPrepController : BaseControllerWithDevOpsOrAdvancedSupportPolicy
    {
        private readonly IPeriodService _periodService;
        private readonly INCSPeriodEndService _periodEndService;
        private readonly IStateService _stateService;

        public PeriodEndPrepController(
            IPeriodService periodService,
            INCSPeriodEndService periodEndService,
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
        public async Task<IActionResult> Index(int? collectionYear, int? period, CancellationToken cancellationToken)
        {
            var currentYearPeriod = await _periodService.ReturnPeriod(CollectionTypes.NCS, cancellationToken);
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

            string state = await _periodEndService.GetPrepStateAsync(model.Year, model.Period, CollectionTypes.NCS, cancellationToken);
            model.PeriodEndPrepModel = _stateService.GetPrepState(state);

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
            await _periodEndService.ReSubmitFailedJobAsync(jobId);

            return RedirectToAction("Index", new { collectionYear, period });
        }

        [HttpPost("startPeriodEnd")]
        public IActionResult StartPeriodEnd(int collectionYear, int period)
        {
            return RedirectToAction("Index", "PeriodEnd", new { collectionYear, period });
        }
    }
}