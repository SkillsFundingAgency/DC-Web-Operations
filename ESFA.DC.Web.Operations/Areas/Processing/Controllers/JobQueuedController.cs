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
    [Route(AreaNames.Processing + "/jobQueued")]
    public class JobQueuedController : BaseController
    {
        IJobQueuedService _jobQueuedService;
        ILogger _logger;

        public JobQueuedController(IJobQueuedService jobQueuedService, ILogger logger, TelemetryClient telemetryClient)
            : base(logger, telemetryClient)
        {
            _jobQueuedService = jobQueuedService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            return View((object)await _jobQueuedService.GetJobsThatAreQueued());
        }
    }
}