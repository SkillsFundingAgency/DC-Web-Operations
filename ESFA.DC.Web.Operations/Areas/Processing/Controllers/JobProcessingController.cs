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
    [Route(AreaNames.Processing + "/jobProcessing")]
    public class JobProcessingController : BaseController
    {
        IJobProcessingService _jobProcessingService;
        ILogger _logger;

        public JobProcessingController(IJobProcessingService jobProcessingService, ILogger logger, TelemetryClient telemetryClient)
            : base(logger, telemetryClient)
        {
            _jobProcessingService = jobProcessingService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            return View("Index", await _jobProcessingService.GetJobsThatAreProcessing());
        }
    }
}