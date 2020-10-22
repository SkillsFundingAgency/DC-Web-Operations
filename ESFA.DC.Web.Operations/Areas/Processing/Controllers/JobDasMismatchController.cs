using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Jobs.Model.Processing.DasMismatch;
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
    [Route(AreaNames.Processing + "/jobDasMismatch")]
    public class JobDasMismatchController : BaseController
    {
        private readonly IJobDasMismatchService _jobDasMisMatchService;
        private readonly IJsonSerializationService _jsonSerializationService;

        public JobDasMismatchController(
            IJobDasMismatchService jobDasMismatchService,
            ILogger logger,
            TelemetryClient telemetryClient,
            IJsonSerializationService jsonSerializationService)
            : base(logger, telemetryClient)
        {
            _jobDasMisMatchService = jobDasMismatchService;
            _jsonSerializationService = jsonSerializationService;
        }

        public async Task<IActionResult> Index(int? collectionYear, CancellationToken cancellationToken)
        {
            var model = _jsonSerializationService.Deserialize<JobProcessingModel<DasMismatchLookupModel>>(await _jobDasMisMatchService.GetDasMismatches(cancellationToken));
            model.CollectionYear = collectionYear;

            return View("Index", model);
        }
    }
}