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
    [Route(AreaNames.Processing + "/jobConcern")]
    public class JobConcernController : BaseController
    {
        private readonly IJobConcernService _jobConcernService;
        private readonly ILogger _logger;

        public JobConcernController(IJobConcernService jobConcernService, ILogger logger, TelemetryClient telemetryClient)
            : base(logger, telemetryClient)
        {
            _jobConcernService = jobConcernService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            return View("Index", await _jobConcernService.GetJobsThatAreConcern());
        }
    }
}