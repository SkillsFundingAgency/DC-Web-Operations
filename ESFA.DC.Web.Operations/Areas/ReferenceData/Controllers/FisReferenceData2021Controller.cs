using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Controllers;
using ESFA.DC.Web.Operations.Extensions;
using ESFA.DC.Web.Operations.Interfaces.Collections;
using ESFA.DC.Web.Operations.Interfaces.ReferenceData;
using ESFA.DC.Web.Operations.Interfaces.Storage;
using ESFA.DC.Web.Operations.Models.ReferenceData;
using ESFA.DC.Web.Operations.Utils;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.Web.Operations.Areas.ReferenceData.Controllers
{
    [Area(AreaNames.ReferenceData)]
    [Route(AreaNames.ReferenceData + "/fisReferenceData2021")]
    public class FisReferenceData2021Controller : BaseReferenceDataController
    {
        private const string CollectionName = CollectionNames.FisReferenceData2021;
        private const int Period = 0;

        private readonly IReferenceDataService _referenceDataService;
        private readonly IReferenceDataProcessService _fisReferenceDataService;
        private readonly ICollectionsService _collectionsService;

        public FisReferenceData2021Controller(
            IStorageService storageService,
            ILogger logger,
            TelemetryClient telemetryClient,
            IReferenceDataService referenceDataService,
            IReferenceDataProcessService fisReferenceDataService,
            ICollectionsService collectionsService,
            IFileNameValidationServiceProvider fileNameValidationServiceProvider)
            : base(storageService, logger, telemetryClient, fileNameValidationServiceProvider)
        {
            _referenceDataService = referenceDataService;
            _fisReferenceDataService = fisReferenceDataService;
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

        [Route("getReportFile/{collectionName}/{fileName}/{jobId?}")]
        public async Task<FileResult> GetCollectionReportFileAsync(string collectionName, string fileName, long? jobId, CancellationToken cancellationToken)
        {
            return await GetReportFileAsync(collectionName, fileName, jobId, cancellationToken);
        }

        private async Task<ReferenceDataViewModel> RefreshModelData(CancellationToken cancellationToken)
        {
            var collection = _collectionsService.GetReferenceDataCollection(CollectionName);

            var model = await _fisReferenceDataService.GetProcessOutputsForCollectionAsync(
                Utils.Constants.ReferenceDataStorageContainerName,
                collection.CollectionName,
                collection.ReportName,
                FileNameExtensionConsts.CSV,
                collection.FileNameFormat,
                collection.FileFormat,
                15,
                cancellationToken: cancellationToken);

            model.CollectionDisplayName = collection.DisplayName;
            model.HubName = collection.HubName;
            model.FileExtension = collection.FileFormat;

            return model;
        }
    }
}
