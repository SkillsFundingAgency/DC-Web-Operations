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
    [Route(AreaNames.PeriodEndILR + "/validityPeriod")]
    public class ValidityPeriodController : BaseControllerWithDevOpsOrAdvancedSupportPolicy
    {
        private const string CollectionType = CollectionTypes.ILR;
        private readonly IPeriodEndService _periodEndService;
        private readonly IPeriodService _periodService;
        private readonly IStateService _stateService;

        public ValidityPeriodController(
            IPeriodEndService periodEndService,
            IPeriodService periodService,
            IStateService stateService,
            ILogger logger,
            TelemetryClient telemetryClient)
            : base(logger, telemetryClient)
        {
            _periodEndService = periodEndService;
            _periodService = periodService;
            _stateService = stateService;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var period = await _periodService.ReturnPeriod(CollectionType, cancellationToken);

            var years = await _periodService.GetValidityYearsAsync(CollectionType, null, cancellationToken);

            var stateString = await _periodEndService.GetPrepStateAsync(period.Year, period.Period, CollectionType, cancellationToken);
            var state = _stateService.GetPrepState(stateString);

            var model = new ValidityPeriodViewModel
            {
                Year = period.Year ?? 0,
                Period = period.Period,
                PeriodEndInProgress = state.State.PeriodEndStarted && !state.State.PeriodEndFinished,
                AllYears = years
            };

            return View(model);
        }
    }
}