using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Controllers;
using ESFA.DC.Web.Operations.Interfaces.Processing;
using ESFA.DC.Web.Operations.Utils;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.Web.Operations.Areas.Processing.Controllers
{
    [Area(AreaNames.Processing)]
    [Route(AreaNames.Processing + "/jobProvidersReturnedCurrentPeriod")]
    public class JobProvidersReturnedCurrentPeriodController : BaseController
    {
        private readonly IJobProvidersReturnedCurrentPeriodService _JobProvidersReturnedCurrentPeriodService;

        public JobProvidersReturnedCurrentPeriodController(
            IJobProvidersReturnedCurrentPeriodService jobProvidersReturnedCurrentPeriodService,
            ILogger logger,
            TelemetryClient telemetryClient)
            : base(logger, telemetryClient)
        {
            _JobProvidersReturnedCurrentPeriodService = jobProvidersReturnedCurrentPeriodService;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            return View("Index", await _JobProvidersReturnedCurrentPeriodService.GetProvidersReturnedCurrentPeriodAsync(cancellationToken));
        }
    }
}