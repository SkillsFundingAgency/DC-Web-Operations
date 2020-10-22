using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Jobs.Model.Processing.JobsSlowFile;
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
    [Route(AreaNames.Processing + "/jobSlowFile")]
    public class JobSlowFileController : BaseController
    {
        private readonly IJobSlowFileService _jobSlowFileService;
        private readonly IJsonSerializationService _jsonSerializationService;

        public JobSlowFileController(
            IJobSlowFileService jobSlowFileService,
            ILogger logger,
            TelemetryClient telemetryClient,
            IJsonSerializationService jsonSerializationService)
            : base(logger, telemetryClient)
        {
            _jobSlowFileService = jobSlowFileService;
            _jsonSerializationService = jsonSerializationService;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var model = _jsonSerializationService.Deserialize<JobProcessingModel<JobSlowFileLookupModel>>(await _jobSlowFileService.GetJobsThatAreSlowFile(cancellationToken));

            return View("Index", model);
        }
    }
}