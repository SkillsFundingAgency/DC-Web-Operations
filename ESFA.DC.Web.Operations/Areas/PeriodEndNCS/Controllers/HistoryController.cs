using System;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Areas.PeriodEndNCS.Models;
using ESFA.DC.Web.Operations.Constants.Authorization;
using ESFA.DC.Web.Operations.Controllers;
using ESFA.DC.Web.Operations.Interfaces.PeriodEnd;
using ESFA.DC.Web.Operations.Utils;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.Web.Operations.Areas.PeriodEndNCS.Controllers
{
    [Area(AreaNames.PeriodEndNCS)]
    [Route(AreaNames.PeriodEndNCS + "/periodEndHistory")]
    public class HistoryController : BaseControllerWithDevOpsOrAdvancedSupportPolicy
    {
        private readonly IPeriodService _periodService;
        private readonly INCSHistoryService _ncsHistoryService;

        public HistoryController(IPeriodService periodService, INCSHistoryService ncsHistoryService, ILogger logger, TelemetryClient telemetryClient)
            : base(logger, telemetryClient)
        {
            _periodService = periodService;
            _ncsHistoryService = ncsHistoryService;
        }

        public async Task<IActionResult> Index(int? collectionYear, CancellationToken cancellationToken)
        {
            var currentYearPeriod = await _periodService.ReturnPeriod(CollectionTypes.NCS, cancellationToken);
            if (currentYearPeriod.Year == null)
            {
                throw new Exception($"Return period {currentYearPeriod.Period} has no year.");
            }

            var model = new HistoryViewModel
            {
                Year = collectionYear ?? currentYearPeriod.Year.Value
            };

            model.PeriodHistories = await _ncsHistoryService.GetHistoryDetails(model.Year);
            model.CollectionYears = await _ncsHistoryService.GetCollectionYears();

            return View(model);
        }
    }
}