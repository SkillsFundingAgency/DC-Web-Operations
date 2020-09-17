using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Controllers;
using ESFA.DC.Web.Operations.Extensions;
using ESFA.DC.Web.Operations.Interfaces.Collections;
using ESFA.DC.Web.Operations.Interfaces.ReferenceData;
using ESFA.DC.Web.Operations.Models.ReferenceData;
using ESFA.DC.Web.Operations.Utils;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.Web.Operations.Areas.ReferenceData.Controllers
{
    [Area(AreaNames.ReferenceData)]
    [Route(AreaNames.ReferenceData + "/fisReferenceData")]
    public class FisReferenceData2021Controller : BaseControllerWithDevOpsOrAdvancedSupportPolicy
    {
        private const string CollectionName = CollectionNames.FisReferenceData2021;
        private const int Period = 0;

        private readonly IReferenceDataService _referenceDataService;
        private readonly ICollectionsService _collectionsService;

        public FisReferenceData2021Controller(
            ILogger logger,
            TelemetryClient telemetryClient,
            IReferenceDataService referenceDataService,
            ICollectionsService collectionsService)
            : base(logger, telemetryClient)
        {
            _referenceDataService = referenceDataService;
            _collectionsService = collectionsService;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            return View("Index", await RefreshModelData(cancellationToken));
        }

        [HttpPost]
        public async Task<IActionResult> Post(ReferenceDataViewModel model, CancellationToken cancellationToken)
        {
            await _referenceDataService.SubmitJobAsync(Period, model.ReferenceDataCollectionName, User.Name(), User.Email(), cancellationToken);

            return View("Index", await RefreshModelData(cancellationToken));
        }

        private async Task<ReferenceDataViewModel> RefreshModelData(CancellationToken cancellationToken)
        {
            var collection = _collectionsService.GetReferenceDataCollection(CollectionName);

            var model = await _referenceDataService.GetSubmissionsPerCollectionAsync(
                Utils.Constants.ReferenceDataStorageContainerName,
                collection.CollectionName,
                collection.ReportName,
                cancellationToken: cancellationToken);

            model.CollectionDisplayName = collection.DisplayName;
            model.HubName = collection.HubName;
            model.FileExtension = collection.FileFormat;

            return model;
        }
    }
}
