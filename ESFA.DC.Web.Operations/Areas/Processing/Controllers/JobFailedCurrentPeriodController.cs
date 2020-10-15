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
        private readonly IJobFailedCurrentPeriodService _jobFailedCurrentPeriodService;

        public JobFailedCurrentPeriodController(
            IJobFailedCurrentPeriodService jobFailedCurrentPeriodService,
            ILogger logger,
            TelemetryClient telemetryClient)
            : base(logger, telemetryClient)
        {
            _jobFailedCurrentPeriodService = jobFailedCurrentPeriodService;
        }

        public async Task<IActionResult> Index(int? collectionYear, CancellationToken cancellationToken)
        {
            return View("Index", await _jobFailedCurrentPeriodService.GetJobsFailedCurrentPeriodAsync(collectionYear, cancellationToken));
        }
    }
}