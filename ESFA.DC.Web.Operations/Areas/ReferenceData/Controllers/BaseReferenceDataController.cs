using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Constants;
using ESFA.DC.Web.Operations.Controllers;
using ESFA.DC.Web.Operations.Interfaces;
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

        public BaseReferenceDataController(
            IStorageService storageService,
            ILogger logger,
            TelemetryClient telemetryClient)
            : base(logger, telemetryClient)
        {
            _storageService = storageService;
            _logger = logger;
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
            IFileNameValidationService fileNameValidationService,
            string rawFileName,
            long? fileLength,
            CancellationToken cancellationToken)
        {
            if (fileNameValidationService == null)
            {
                throw new ArgumentNullException(nameof(fileNameValidationService));
            }

            var fileName = Path.GetFileName(rawFileName);
            var validationResult = await fileNameValidationService.ValidateFileNameAsync(fileName, fileLength, cancellationToken);

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