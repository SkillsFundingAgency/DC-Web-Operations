using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
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
    [Route(AreaNames.ReferenceData + "/referenceDataProcess")]
    public class ReferenceDataProcessController : BaseReferenceDataController
    {
        private const int Period = 0;

        private readonly IReferenceDataService _referenceDataService;
        private readonly IReferenceDataProcessService _referenceDataProcessService;
        private readonly ICollectionsService _collectionsService;

        public ReferenceDataProcessController(
            IStorageService storageService,
            ILogger logger,
            TelemetryClient telemetryClient,
            IReferenceDataService referenceDataService,
            IReferenceDataProcessService referenceDataProcessService,
            ICollectionsService collectionsService,
            IFileNameValidationServiceProvider fileNameValidationServiceProvider)
            : base(storageService, logger, telemetryClient, fileNameValidationServiceProvider)
        {
            _referenceDataService = referenceDataService;
            _referenceDataProcessService = referenceDataProcessService;
            _collectionsService = collectionsService;
        }

        [Route("{collectionName}")]
        public async Task<IActionResult> Index(string collectionName, CancellationToken cancellationToken)
        {
            return View("Index", await RefreshModelData(collectionName, cancellationToken));
        }

        [HttpPost]
        public async Task<IActionResult> Post(ReferenceDataViewModel model, CancellationToken cancellationToken)
        {
            await _referenceDataService.SubmitJobAsync(Period, model.ReferenceDataCollectionName, User.Name(), User.Email(), cancellationToken);

            return RedirectToAction("Index", "ReferenceDataProcess", new { collectionName = model.ReferenceDataCollectionName });
        }

        [Route("getCollectionReportFileAsync/{collectionName}/{fileName}/{jobId?}")]
        public async Task<FileResult> GetCollectionReportFileAsync(string collectionName, string fileName, long? jobId, CancellationToken cancellationToken)
        {
            return await GetReportFileAsync(collectionName, fileName, jobId, cancellationToken);
        }

        private async Task<ReferenceDataViewModel> RefreshModelData(string collectionName, CancellationToken cancellationToken)
        {
            var collection = _collectionsService.GetReferenceDataCollection(collectionName);

            var model = await _referenceDataProcessService.GetProcessOutputsForCollectionAsync(
                Utils.Constants.ReferenceDataStorageContainerName,
                collection.CollectionName,
                collection.ReportName,
                FileNameExtensionConsts.CSV,
                collection.FileNameFormat,
                collection.FileFormat,
                cancellationToken: cancellationToken);

            model.CollectionDisplayName = collection.DisplayName;
            model.HubName = collection.HubName;
            model.FileExtension = collection.FileFormat;

            return model;
        }
    }
}
