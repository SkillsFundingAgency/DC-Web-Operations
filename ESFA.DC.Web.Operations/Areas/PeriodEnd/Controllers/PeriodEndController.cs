﻿using System.Threading;
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
    [Route(AreaNames.PeriodEnd + "/periodEnd")]
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

        [HttpGet]
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var ilrPeriodEndInfo = _periodService.ReturnPeriod(CollectionTypes.ILR, cancellationToken);
            var ncsPeriodEndInfo = _periodService.ReturnPeriod(CollectionTypes.NCS, cancellationToken);
            var allfPeriodEndInfo = _periodService.ReturnPeriod(CollectionTypes.ALLF, cancellationToken);

            await Task.WhenAll(ilrPeriodEndInfo, ncsPeriodEndInfo, allfPeriodEndInfo);

            var model = new PeriodEndViewModel
            {
                ILRPeriodEndInfo = ilrPeriodEndInfo.Result,
                NCSPeriodEndInfo = ncsPeriodEndInfo.Result,
                ALLFPeriodEndInfo = allfPeriodEndInfo.Result
            };

            return View(model);
        }
    }
}