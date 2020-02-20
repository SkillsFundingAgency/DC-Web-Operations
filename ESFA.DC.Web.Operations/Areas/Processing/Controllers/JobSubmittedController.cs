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
    [Route(AreaNames.Processing + "/jobSubmitted")]
    public class JobSubmittedController : BaseController
    {
        IJobSubmittedService _jobSubmittedService;
        ILogger _logger;

        public JobSubmittedController(IJobSubmittedService jobSubmittedService, ILogger logger, TelemetryClient telemetryClient)
            : base(logger, telemetryClient)
        {
            _jobSubmittedService = jobSubmittedService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            return View((object)await _jobSubmittedService.GetJobsThatAreSubmitted());
        }
    }
}