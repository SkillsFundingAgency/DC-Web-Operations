﻿using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Extensions;
using ESFA.DC.Web.Operations.Interfaces.Collections;
using ESFA.DC.Web.Operations.Interfaces.ReferenceData;
using ESFA.DC.Web.Operations.Interfaces.Storage;
using ESFA.DC.Web.Operations.Models.Enums;
using ESFA.DC.Web.Operations.Models.ReferenceData;
using ESFA.DC.Web.Operations.Utils;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.Web.Operations.Areas.ReferenceData.Controllers
{
    [Area(AreaNames.ReferenceData)]
    [Route(AreaNames.ReferenceData + "/referenceData")]
    public class ReferenceDataController : BaseReferenceDataController
    {
        private readonly IReferenceDataService _referenceDataService;
        private readonly ICollectionsService _collectionsService;

        public ReferenceDataController(
            IStorageService storageService,
            ILogger logger,
            TelemetryClient telemetryClient,
            IReferenceDataService referenceDataService,
            ICollectionsService collectionsService,
            IFileNameValidationServiceProvider fileNameValidationServiceProvider)
            : base(storageService, logger, telemetryClient, fileNameValidationServiceProvider)
        {
            _referenceDataService = referenceDataService;
            _collectionsService = collectionsService;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var model = await _referenceDataService.GetLatestReferenceDataJobs(cancellationToken);

            return View("Index", model);
        }

        [Route("{collectionName}")]
        public async Task<IActionResult> Index(string collectionName, CancellationToken cancellationToken)
        {
            var collection = _collectionsService.GetReferenceDataCollection(collectionName);

            var model = await _referenceDataService.GetSubmissionsPerCollectionAsync(
                Utils.Constants.ReferenceDataStorageContainerName,
                collection.CollectionName,
                collection.ReportName,
                cancellationToken: cancellationToken);

            model.CollectionDisplayName = collection.DisplayName;
            model.HubName = collection.HubName;
            model.FileExtension = collection.FileFormat;

            return View("FileUpload", model);
        }

        [RequestSizeLimit(524_288_000)]
        [AutoValidateAntiforgeryToken]
        [HttpPost]
        public async Task<IActionResult> Post([FromForm] IFormFile file, ReferenceDataViewModel model, CancellationToken cancellationToken)
        {
            if (file == null)
            {
                return RedirectToAction("Index", "ReferenceData", new { collectionName = model.ReferenceDataCollectionName });
            }

            var collection = _collectionsService.GetReferenceDataCollection(model.ReferenceDataCollectionName);

            var validationResult = await ValidateFileName(
                collection,
                file.FileName,
                file.Length,
                cancellationToken);

            if (validationResult.ValidationResult != FileNameValidationResult.Valid)
            {
                return View("FileUpload", model);
            }

            await _referenceDataService.SubmitJobAsync(Period, model.ReferenceDataCollectionName, User.Name(), User.Email(), file, cancellationToken);

            return RedirectToAction("Index", "ReferenceData", new { collectionName = model.ReferenceDataCollectionName });
        }

        [Route("getCollectionReportFileAsync/{collectionName}/{fileName}/{jobId?}")]
        public async Task<FileResult> GetCollectionReportFileAsync(string collectionName, string fileName, long? jobId, CancellationToken cancellationToken)
        {
            return await GetReportFileAsync(collectionName, fileName, jobId, cancellationToken);
        }
    }
}