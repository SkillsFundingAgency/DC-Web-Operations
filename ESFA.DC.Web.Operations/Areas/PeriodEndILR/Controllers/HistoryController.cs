using System;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Areas.PeriodEndILR.Models;
using ESFA.DC.Web.Operations.Constants.Authorization;
using ESFA.DC.Web.Operations.Controllers;
using ESFA.DC.Web.Operations.Interfaces.PeriodEnd;
using ESFA.DC.Web.Operations.Utils;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.Web.Operations.Areas.PeriodEndILR.Controllers
{
    [Area(AreaNames.PeriodEndILR)]
    [Route(AreaNames.PeriodEndILR + "/periodEndHistory")]
    public class HistoryController : BaseControllerWithOpsPolicy
    {
        private readonly IPeriodService _periodService;
        private readonly IHistoryService _historyService;

        public HistoryController(IPeriodService periodService, IHistoryService historyService, ILogger logger, TelemetryClient telemetryClient)
            : base(logger, telemetryClient)
        {
            _periodService = periodService;
            _historyService = historyService;
        }

        public async Task<IActionResult> Index(int? collectionYear)
        {
            var currentYearPeriod = await _periodService.ReturnPeriod();
            if (currentYearPeriod.Year == null)
            {
                throw new Exception($"Return period {currentYearPeriod.Period} has no year.");
            }

            var model = new HistoryViewModel
            {
                Year = collectionYear ?? currentYearPeriod.Year.Value
            };

            model.PeriodHistories = await _historyService.GetHistoryDetails(model.Year);
            model.CollectionYears = await _historyService.GetCollectionYears();

            return View(model);
        }
    }
}