﻿using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Features.Indexed;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Constants;
using ESFA.DC.Web.Operations.Extensions;
using ESFA.DC.Web.Operations.Interfaces;
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
    [Route(AreaNames.ReferenceData + "/conditionOfFundingRemoval")]
    public class ConditionOfFundingRemovalController : BaseReferenceDataController
    {
        private readonly IReferenceDataService _referenceDataService;
        private readonly IFileNameValidationService _fileNameValidationService;
        private readonly ILogger _logger;

        public ConditionOfFundingRemovalController(
            IStorageService storageService,
            ILogger logger,
            TelemetryClient telemetryClient,
            IReferenceDataService referenceDataService,
            IIndex<string, IFileNameValidationService> fileNameValidationService)
            : base(storageService, logger, telemetryClient)
        {
            _logger = logger;
            _referenceDataService = referenceDataService;
            _fileNameValidationService = fileNameValidationService[CollectionNames.ReferenceDataConditionsOfFundingRemoval];
        }

        public async Task<IActionResult> Index()
        {
            var cancellationToken = CancellationToken.None;

            var model = await _referenceDataService.GetSubmissionsPerCollectionAsync(
                Utils.Constants.ReferenceDataStorageContainerName,
                CollectionNames.ReferenceDataConditionsOfFundingRemoval,
                ReportTypes.ConditionOfFundingRemovalReportName,
                cancellationToken: cancellationToken);

            return View(model);
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

            const int period = 0;
            var fileName = Path.GetFileName(file?.FileName);
            var validationResult = await _fileNameValidationService.ValidateFileNameAsync(CollectionNames.ReferenceDataConditionsOfFundingRemoval, fileName, file?.Length, cancellationToken);

            if (validationResult.ValidationResult != FileNameValidationResult.Valid)
            {
                ModelState.AddModelError(ErrorMessageKeys.Submission_FileFieldKey, validationResult.FieldError);
                ModelState.AddModelError(ErrorMessageKeys.ErrorSummaryKey, validationResult.SummaryError);

                _logger.LogWarning($"User uploaded invalid file with name :{fileName}");
                return View();
            }

            await _referenceDataService.SubmitJob(period, CollectionNames.ReferenceDataConditionsOfFundingRemoval, User.Name(), User.Email(), file, CancellationToken.None);

            return RedirectToAction("Index");
        }

        [Route("getReportFile/{fileName}/{jobId?}")]
        public async Task<FileResult> GetReportFileAsync(string fileName, long? jobId, CancellationToken cancellationToken)
        {
            return await GetReportFileAsync(CollectionNames.ReferenceDataConditionsOfFundingRemoval, fileName, jobId, cancellationToken);
        }
    }
}