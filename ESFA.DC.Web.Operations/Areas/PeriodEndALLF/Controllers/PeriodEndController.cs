using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Constants;
using ESFA.DC.Web.Operations.Extensions;
using ESFA.DC.Web.Operations.Interfaces.PeriodEnd;
using ESFA.DC.Web.Operations.Interfaces.Storage;
using ESFA.DC.Web.Operations.Utils;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.Web.Operations.Areas.PeriodEndALLF.Controllers
{
    [Area(AreaNames.PeriodEndALLF)]
    [Route(AreaNames.PeriodEndALLF + "/periodEnd")]
    public class PeriodEndController : BaseALLFController
    {
        private readonly IALLFPeriodEndService _periodEndService;
        private readonly IPeriodService _periodService;
        private readonly IStateService _stateService;
        private readonly IStorageService _storageService;
        private readonly ILogger _logger;

        public PeriodEndController(
            IALLFPeriodEndService periodEndService,
            IStorageService storageService,
            ILogger logger,
            TelemetryClient telemetryClient)
            : base(storageService, logger, telemetryClient)
        {
            _periodEndService = periodEndService;
            _periodService = periodService;
            _stateService = stateService;
            _storageService = storageService;
            _logger = logger;
        }

        [HttpGet("{collectionYear?}/{period?}")]
        public async Task<IActionResult> Index(int? collectionYear, int? period)
        {
            var cancellationToken = CancellationToken.None;

            var model = await _periodEndService.GetPathState(collectionYear, period, cancellationToken);

            return View(model);
        }

        [RequestSizeLimit(524_288_000)]
        [AutoValidateAntiforgeryToken]
        [HttpPost("{collectionYear?}/{period?}")]
        public async Task<IActionResult> Index([FromForm]int collectionYear, [FromForm]int period, IFormFile file)
        {
            var cancellationToken = CancellationToken.None;
            if (file != null)
            {
                await _periodEndService.InitialisePeriodEndAsync(collectionYear, period, CollectionTypes.ALLF, cancellationToken);

                await _periodEndService.SubmitJob(period, CollectionNames.ALLFCollection, User.Name(), User.Email(), file, cancellationToken);
            }

            return RedirectToAction("Index", new { collectionYear, period });
        }
    }
}