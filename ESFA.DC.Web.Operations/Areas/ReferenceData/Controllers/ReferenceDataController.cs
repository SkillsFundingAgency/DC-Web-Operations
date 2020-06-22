using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.ReferenceData;
using ESFA.DC.Web.Operations.Interfaces.Storage;
using ESFA.DC.Web.Operations.Utils;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.Web.Operations.Areas.ReferenceData.Controllers
{
    [Area(AreaNames.ReferenceData)]
    [Route(AreaNames.ReferenceData + "/referenceData")]
    public class ReferenceDataController : BaseReferenceDataController
    {
        private readonly IReferenceDataService _referenceDataService;
        private readonly IJobService _jobService;

        public ReferenceDataController(
            IStorageService storageService,
            ILogger logger,
            TelemetryClient telemetryClient,
            IReferenceDataService referenceDataService,
            IJobService jobService)
            : base(storageService, logger, telemetryClient)
        {
            _referenceDataService = referenceDataService;
            _jobService = jobService;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var model = await _referenceDataService.GetLatestReferenceDataJobs(cancellationToken);

            return View("Index", model);
        }
    }
}