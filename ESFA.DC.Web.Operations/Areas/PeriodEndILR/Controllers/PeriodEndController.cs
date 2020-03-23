using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Areas.PeriodEndILR.Models;
using ESFA.DC.Web.Operations.Controllers;
using ESFA.DC.Web.Operations.Interfaces.PeriodEnd;
using ESFA.DC.Web.Operations.Utils;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.Web.Operations.Areas.PeriodEndILR.Controllers
{
    [Area(AreaNames.PeriodEndILR)]
    [Route(AreaNames.PeriodEndILR + "/periodEnd")]
    public class PeriodEndController : BaseControllerWithOpsPolicy
    {
        private readonly IPeriodService _periodService;
        private readonly IPeriodEndService _periodEndService;
        private readonly IEmailService _emailService;
        private readonly IStateService _stateService;

        public PeriodEndController(
            IPeriodService periodService,
            IPeriodEndService periodEndService,
            IEmailService emailService,
            IStateService stateService,
            ILogger logger,
            TelemetryClient telemetryClient)
            : base(logger, telemetryClient)
        {
            _periodService = periodService;
            _periodEndService = periodEndService;
            _emailService = emailService;
            _stateService = stateService;
        }

        [HttpGet("{collectionYear?}/{period?}")]
        public async Task<IActionResult> Index(int? collectionYear, int? period)
        {
            var currentYearPeriod = await _periodService.ReturnPeriod();
            currentYearPeriod.Year = currentYearPeriod.Year ?? 0;
            PeriodEndViewModel model;

            if (collectionYear != null && period != null)
            {
                model = await ShowPath(collectionYear.Value, period.Value);
                model.Year = collectionYear.Value;
                model.Period = period.Value;
            }
            else
            {
                model = await ShowPath(currentYearPeriod.Year.Value, currentYearPeriod.Period);
                model.Year = currentYearPeriod.Year.Value;
                model.Period = currentYearPeriod.Period;
            }

            var isCurrentPeriodSelected = currentYearPeriod.Year == model.Year && currentYearPeriod.Period == model.Period;
            model.IsCurrentPeriod = isCurrentPeriodSelected;
            model.CollectionClosed = isCurrentPeriodSelected && currentYearPeriod.PeriodClosed;

            return View(model);
        }

        private async Task<PeriodEndViewModel> ShowPath(int collectionYear, int period)
        {
            var pathItemStates = await _periodEndService.GetPathItemStates(collectionYear, period);
            var state = _stateService.GetMainState(pathItemStates);

            var pathModel = new PeriodEndViewModel
            {
                Period = period,
                Year = collectionYear,
                Paths = state.Paths,
                CollectionClosed = state.CollectionClosed,
                PeriodEndStarted = state.PeriodEndStarted,
                McaReportsReady = state.McaReportsReady,
                McaReportsPublished = state.McaReportsPublished,
                ProviderReportsReady = state.ProviderReportsReady,
                ProviderReportsPublished = state.ProviderReportsPublished,
                PeriodEndFinished = state.PeriodEndFinished,
                ReferenceDataJobsPaused = state.ReferenceDataJobsPaused
            };

            return pathModel;
        }
    }
}