using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Extensions;
using ESFA.DC.Web.Operations.Interfaces.ReferenceData;
using ESFA.DC.Web.Operations.Interfaces.Storage;
using ESFA.DC.Web.Operations.Models.ReferenceData;
using ESFA.DC.Web.Operations.Utils;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.Web.Operations.Areas.ReferenceData.Controllers
{
    [Area(AreaNames.ReferenceData)]
    [Route(AreaNames.ReferenceData + "/conditionOfFundingRemoval")]
    public class ConditionOfFundingRemovalController : BaseReferenceDataController
    {
        private readonly IReferenceDataService _referenceDataService;

        public ConditionOfFundingRemovalController(
            IStorageService storageService,
            ILogger logger,
            TelemetryClient telemetryClient,
            IReferenceDataService referenceDataService)
            : base(storageService, logger, telemetryClient)
        {
            _referenceDataService = referenceDataService;
        }

        public async Task<IActionResult> Index()
        {
            var cancellationToken = CancellationToken.None;

            var submissions = await _referenceDataService.GetSubmissionsPerCollectionAsync(
                CollectionNames.ReferenceDataConditionsOfFundingRemoval,
                ReportTypes.ConditionOfFundingRemovalReportName,
                cancellationToken);

            var model = new ReferenceDataViewModel();
            model.FileUploads = submissions;

            return View(model);
        }

        [RequestSizeLimit(524_288_000)]
        [AutoValidateAntiforgeryToken]
        [HttpPost]
        public async Task<IActionResult> Index([FromForm] IFormFile file)
        {
            const int period = 0;

            if (file != null)
            {
                await _referenceDataService.SubmitJob(period, CollectionNames.ReferenceDataConditionsOfFundingRemoval, User.Name(), User.Email(), file, CancellationToken.None);
            }

            return View("Index");
        }
    }
}