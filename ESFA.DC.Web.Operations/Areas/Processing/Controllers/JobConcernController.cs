using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Jobs.Model.Processing.JobsConcern;
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
    [Route(AreaNames.Processing + "/jobConcern")]
    public class JobConcernController : BaseController
    {
        private readonly IJobConcernService _jobConcernService;
        private readonly IJsonSerializationService _jsonSerializationService;

        public JobConcernController(
            IJobConcernService jobConcernService,
            ILogger logger,
            TelemetryClient telemetryClient,
            IJsonSerializationService jsonSerializationService)
            : base(logger, telemetryClient)
        {
            _jobConcernService = jobConcernService;
            _jsonSerializationService = jsonSerializationService;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var model = _jsonSerializationService.Deserialize<JobProcessingModel<JobConcernLookupModel>>(await _jobConcernService.GetJobsThatAreConcern(cancellationToken));

            return View("Index", model);
        }
    }
}