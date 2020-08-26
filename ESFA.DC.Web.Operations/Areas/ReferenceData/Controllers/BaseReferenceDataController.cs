using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Constants;
using ESFA.DC.Web.Operations.Controllers;
using ESFA.DC.Web.Operations.Interfaces.Collections;
using ESFA.DC.Web.Operations.Interfaces.ReferenceData;
using ESFA.DC.Web.Operations.Interfaces.Storage;
using ESFA.DC.Web.Operations.Models;
using ESFA.DC.Web.Operations.Models.Enums;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.Web.Operations.Areas.ReferenceData.Controllers
{
    public abstract class BaseReferenceDataController : BaseControllerWithDevOpsOrAdvancedSupportPolicy
    {
        private readonly IStorageService _storageService;
        private readonly ILogger _logger;
        private readonly IFileNameValidationServiceProvider _fileNameValidationServiceProvider;

        public BaseReferenceDataController(
            IStorageService storageService,
            ILogger logger,
            TelemetryClient telemetryClient,
            IFileNameValidationServiceProvider fileNameValidationServiceProvider)
            : base(logger, telemetryClient)
        {
            _storageService = storageService;
            _logger = logger;
            _fileNameValidationServiceProvider = fileNameValidationServiceProvider;
        }

        protected int Period => 0;

        protected async Task<FileResult> GetReportFileAsync(string collectionName, string fileName, long? jobId, CancellationToken cancellationToken)
        {
            var reportFile = jobId != null;
            fileName = reportFile ? $@"{collectionName}\{jobId}\{fileName}" : $@"{collectionName}\{fileName}";
            try
            {
                var blobStream = await _storageService.GetFile(Utils.Constants.ReferenceDataStorageContainerName, fileName, cancellationToken);

                return new FileStreamResult(blobStream, _storageService.GetMimeTypeFromFileName(fileName))
                {
                    FileDownloadName = fileName
                };
            }
            catch (Exception e)
            {
                _logger.LogError($"Download report failed for file name : {fileName}", e);
                throw;
            }
        }

        protected virtual async Task<FileNameValidationResultModel> ValidateFileName(
            ICollection collection,
            string rawFileName,
            long? fileLength,
            CancellationToken cancellationToken)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            var fileName = Path.GetFileName(rawFileName);
            var fileNameValidationService = _fileNameValidationServiceProvider.GetFileNameValidationService(collection.CollectionName);
            var validationResult = await fileNameValidationService.ValidateFileNameAsync(collection.CollectionName, fileName, collection.FileNameFormat, fileLength, cancellationToken);

            if (validationResult.ValidationResult != FileNameValidationResult.Valid)
            {
                ModelState.AddModelError(ErrorMessageKeys.Submission_FileFieldKey, validationResult.FieldError);
                ModelState.AddModelError(ErrorMessageKeys.ErrorSummaryKey, validationResult.SummaryError);

                _logger.LogWarning($"User uploaded invalid file with name :{fileName}");
            }

            return validationResult;
        }
    }
}