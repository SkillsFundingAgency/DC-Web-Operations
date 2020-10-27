using System.Threading.Tasks;
using ESFA.DC.Jobs.Model.Processing.JobsProcessing;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Web.Operations.Controllers;
using ESFA.DC.Web.Operations.Interfaces.Processing;
using ESFA.DC.Web.Operations.Models.Processing;
using ESFA.DC.Web.Operations.Utils;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.Web.Operations.Areas.Processing.Controllers
{
    [Area(AreaNames.Processing)]
    [Route(AreaNames.Processing + "/jobProcessing")]
    public class JobProcessingController : BaseController
    {
        private readonly IJobProcessingService _jobProcessingService;
        private readonly IJsonSerializationService _jsonSerializationService;

        public JobProcessingController(
            IJobProcessingService jobProcessingService,
            ILogger logger,
            TelemetryClient telemetryClient,
            IJsonSerializationService jsonSerializationService)
            : base(logger, telemetryClient)
        {
            _jobProcessingService = jobProcessingService;
            _jsonSerializationService = jsonSerializationService;
        }

        public async Task<IActionResult> Index()
        {
            var model = _jsonSerializationService.Deserialize<JobProcessingModel<JobProcessingLookupModel>>(await _jobProcessingService.GetJobsThatAreProcessing());
            return View("Index", model);
        }
    }
}