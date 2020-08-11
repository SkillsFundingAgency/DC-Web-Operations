using System;
using System.Collections.Generic;
using System.Linq;
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
    [Route(AreaNames.PeriodEndILR + "/periodEndHistory")]
    public class HistoryController : BaseControllerWithDevOpsOrAdvancedSupportPolicy
    {
        private readonly IPeriodService _periodService;
        private readonly IILRHistoryService _ilrHistoryService;

        public HistoryController(IPeriodService periodService, IILRHistoryService ilrIlrHistoryService, ILogger logger, TelemetryClient telemetryClient)
            : base(logger, telemetryClient)
        {
            _periodService = periodService;
            _ilrHistoryService = ilrIlrHistoryService;
        }

        public async Task<IActionResult> Index(int? collectionYear, CancellationToken cancellationToken)
        {
            var collectionYears = await _ilrHistoryService.GetCollectionYears(cancellationToken);

            if (collectionYears == null
                || !collectionYears.Any())
            {
                throw new Exception($"No historic period ends present");
            }

            collectionYears = collectionYears.OrderBy(s => s);

            var model = new HistoryViewModel
            {
                Year = collectionYear ?? collectionYears.Last(),
                CollectionYears = collectionYears
            };

            model.PeriodHistories = await _ilrHistoryService.GetHistoryDetails(model.Year);
            return View(model);
        }
    }
}