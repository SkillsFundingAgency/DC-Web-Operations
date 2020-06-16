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

        [Route("getReportFile/{fileName}/{period?}/{jobId?}")]
        public async Task<FileResult> GetReportFile(string fileName, int? period, long? jobId)
        {
            var reportFile = period != null && jobId != null;
            fileName = reportFile ? $@"{Utils.Constants.ALLFPeriodPrefix}{period}\{jobId}\{fileName}" : fileName;
            try
            {
                var blobStream = await _storageService.GetFile(Utils.Constants.ALLFStorageContainerName, fileName, CancellationToken.None);

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