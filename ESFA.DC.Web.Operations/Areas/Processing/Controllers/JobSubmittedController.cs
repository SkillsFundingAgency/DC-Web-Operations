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
        private readonly IJobSubmittedService _jobSubmittedService;

        public JobSubmittedController(IJobSubmittedService jobSubmittedService, ILogger logger, TelemetryClient telemetryClient)
            : base(logger, telemetryClient)
        {
            _jobSubmittedService = jobSubmittedService;
        }

        public async Task<IActionResult> Index()
        {
            return View("Index", await _jobSubmittedService.GetJobsThatAreSubmitted());
        }
    }
}