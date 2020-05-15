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
    [Route(AreaNames.Processing + "/jobFailedCurrentPeriod")]
    public class JobFailedCurrentPeriodController : BaseController
    {
        IJobFailedCurrentPeriodService _jobFailedCurrentPeriodService;
        ILogger _logger;

        public JobFailedCurrentPeriodController(
            IJobFailedCurrentPeriodService jobFailedCurrentPeriodService,
            ILogger logger,
            TelemetryClient telemetryClient)
            : base(logger, telemetryClient)
        {
            _jobFailedCurrentPeriodService = jobFailedCurrentPeriodService;
            _logger = logger;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            return View("Index", await _jobFailedCurrentPeriodService.GetJobsFailedCurrentPeriodAsync(cancellationToken));
        }
    }
}