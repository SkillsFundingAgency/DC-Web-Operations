using System;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Areas.PeriodEndALLF.Models;
using ESFA.DC.Web.Operations.Interfaces.PeriodEnd;
using ESFA.DC.Web.Operations.Interfaces.Storage;
using ESFA.DC.Web.Operations.Utils;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.Web.Operations.Areas.PeriodEndALLF.Controllers
{
    [Area(AreaNames.PeriodEndALLF)]
    [Route(AreaNames.PeriodEndALLF + "/history")]
    public class HistoryController : BaseALLFController
    {
        private readonly IPeriodService _periodService;
        private readonly IALLFHistoryService _allfHistoryService;

        public HistoryController(
            IPeriodService periodService,
            IStorageService storageService,
            IALLFHistoryService allfHistoryService,
            ILogger logger,
            TelemetryClient telemetryClient)
            : base(storageService, logger, telemetryClient)
        {
            _periodService = periodService;
            _allfHistoryService = allfHistoryService;
        }

        public async Task<IActionResult> Index(int? collectionYear, CancellationToken cancellationToken)
        {
            var currentYearPeriod = await _periodService.ReturnPeriod(CollectionTypes.ALLF, cancellationToken);

            if (currentYearPeriod.Year == null)
            {
                throw new Exception($"Return period {currentYearPeriod.Period} has no year.");
            }

            var model = new HistoryViewModel
            {
                Year = collectionYear ?? currentYearPeriod.Year.Value
            };

            model.PeriodHistories = await _allfHistoryService.GetHistoryDetails(model.Year, currentYearPeriod.Period, cancellationToken);

            return View(model);
        }
    }
}