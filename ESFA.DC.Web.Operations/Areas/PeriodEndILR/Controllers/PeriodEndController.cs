using System.Globalization;
using System.Threading;
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
    public class PeriodEndController : BaseControllerWithDevOpsOrAdvancedSupportPolicy
    {
        private readonly IPeriodService _periodService;
        private readonly IPeriodEndService _periodEndService;
        private readonly IStateService _stateService;

        public PeriodEndController(
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

        [HttpGet("{collectionYear?}/{period?}/{betaView?}")]
        public async Task<IActionResult> Index(int? collectionYear, int? period, string betaView, CancellationToken cancellationToken)
        {
            var currentYearPeriod = await _periodService.ReturnPeriod(CollectionTypes.ILR, cancellationToken);
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
            var isPreviousPeriod = currentYearPeriod.Year == model.Year && currentYearPeriod.Period == model.Period + 1;

            model.IsPreviousPeriod = isPreviousPeriod;
            model.IsCurrentPeriod = isCurrentPeriodSelected;

            if (betaView?.ToLower(CultureInfo.CurrentUICulture) == "beta")
            {
                return View("IndexBeta", model);
            }

            return View(model);
        }

        private async Task<PeriodEndViewModel> ShowPath(int collectionYear, int period)
        {
            var pathItemStates = await _periodEndService.GetPathItemStatesAsync(collectionYear, period, CollectionTypes.ILR);
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