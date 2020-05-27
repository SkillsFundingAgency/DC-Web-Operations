using System;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Areas.PeriodEndALLF.Models;
using ESFA.DC.Web.Operations.Controllers;
using ESFA.DC.Web.Operations.Interfaces.PeriodEnd;
using ESFA.DC.Web.Operations.Interfaces.Storage;
using ESFA.DC.Web.Operations.Utils;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.Web.Operations.Areas.PeriodEndALLF.Controllers
{
    [Area(AreaNames.PeriodEndALLF)]
    [Route(AreaNames.PeriodEndALLF + "/history")]
    public class HistoryController : BaseControllerWithOpsPolicy
    {
        private readonly IPeriodService _periodService;
        private readonly IStorageService _storageService;
        private readonly IALLFHistoryService _allfHistoryService;
        private readonly ILogger _logger;

        public HistoryController(
            IPeriodService periodService,
            IStorageService storageService,
            IALLFHistoryService allfHistoryService,
            ILogger logger,
            TelemetryClient telemetryClient)
            : base(logger, telemetryClient)
        {
            _periodService = periodService;
            _storageService = storageService;
            _allfHistoryService = allfHistoryService;
            _logger = logger;
        }

        public async Task<IActionResult> Index(int? collectionYear, CancellationToken cancellationToken)
        {
            var currentYearPeriod = await _periodService.ReturnPeriod(CollectionTypes.NCS, cancellationToken);
            if (currentYearPeriod.Year == null)
            {
                throw new Exception($"Return period {currentYearPeriod.Period} has no year.");
            }

            var model = new HistoryViewModel
            {
                Year = collectionYear ?? currentYearPeriod.Year.Value
            };

            model.PeriodHistories = await _allfHistoryService.GetHistoryDetails(model.Year);

            return View(model);
        }

        [Route("getReportFile/{fileName}/{period?}/{jobId?}")]
        public async Task<FileResult> GetReportFile(string fileName, int? period, long? jobId)
        {
            fileName = $@"{Utils.Constants.ALLFPeriodPrefix}{period}\{jobId}\{fileName}";
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