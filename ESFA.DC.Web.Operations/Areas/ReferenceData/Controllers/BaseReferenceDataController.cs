using System;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Controllers;
using ESFA.DC.Web.Operations.Interfaces.Storage;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.Web.Operations.Areas.ReferenceData.Controllers
{
    public class BaseReferenceDataController : BaseControllerWithDevOpsOrAdvancedSupportPolicy
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

        protected async Task<FileResult> GetReportFileAsync(string folderName, string fileName, long? jobId, CancellationToken cancellationToken)
        {
            var reportFile = jobId != null;
            fileName = reportFile ? $@"{folderName}\{jobId}\{fileName}" : $@"{folderName}\{fileName}";
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
    }
}