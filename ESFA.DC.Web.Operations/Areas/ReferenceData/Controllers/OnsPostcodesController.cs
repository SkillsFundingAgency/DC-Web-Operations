﻿using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Extensions;
using ESFA.DC.Web.Operations.Interfaces.ReferenceData;
using ESFA.DC.Web.Operations.Interfaces.Storage;
using ESFA.DC.Web.Operations.Models.Enums;
using ESFA.DC.Web.Operations.Utils;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.Web.Operations.Areas.ReferenceData.Controllers
{
    [Area(AreaNames.ReferenceData)]
    [Route(AreaNames.ReferenceData + "/onsPostcodes")]
    public class OnsPostcodesController : BaseReferenceDataController
    {
        private readonly IReferenceDataService _referenceDataService;
        private readonly IFileNameValidationServiceProvider _fileNameValidationServiceProvider;

        public OnsPostcodesController(
            IStorageService storageService,
            ILogger logger,
            TelemetryClient telemetryClient,
            IReferenceDataService referenceDataService,
            IFileNameValidationServiceProvider fileNameValidationServiceProvider)
            : base(storageService, logger, telemetryClient)
        {
            _referenceDataService = referenceDataService;
            _fileNameValidationServiceProvider = fileNameValidationServiceProvider;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var model = await _referenceDataService.GetSubmissionsPerCollectionAsync(
                    Utils.Constants.ReferenceDataStorageContainerName,
                    CollectionNames.OnsPostcodes,
                    ReportTypes.OnsPostcodesReportName,
                    cancellationToken: cancellationToken);

            return View("Index", model);
        }

        [RequestSizeLimit(524_288_000)]
        [AutoValidateAntiforgeryToken]
        [HttpPost]
        public async Task<IActionResult> Index([FromForm] IFormFile file, CancellationToken cancellationToken)
        {
            if (file == null)
            {
                return RedirectToAction("Index");
            }

            var fileNameValidationService = _fileNameValidationServiceProvider.GetFileNameValidationService(CollectionNames.OnsPostcodes);

            var validationResult = await ValidateFileName(
                fileNameValidationService,
                file.FileName,
                file.Length,
                cancellationToken);

            if (validationResult.ValidationResult != FileNameValidationResult.Valid)
            {
                return View();
            }

            await _referenceDataService.SubmitJob(Period, CollectionNames.OnsPostcodes, User.Name(), User.Email(), file, cancellationToken);

            return RedirectToAction("Index");
        }

        [Route("getReportFile/{collectionName}/{fileName}/{jobId?}")]
        public async Task<FileResult> GetCollectionReportFileAsync(string collectionName, string fileName, long? jobId, CancellationToken cancellationToken)
        {
            return await GetReportFileAsync(collectionName, fileName, jobId, cancellationToken);
        }
    }
}