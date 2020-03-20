using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Areas.PeriodEnd.Models;
using ESFA.DC.Web.Operations.Controllers;
using ESFA.DC.Web.Operations.Interfaces.PeriodEnd;
using ESFA.DC.Web.Operations.Interfaces.Storage;
using ESFA.DC.Web.Operations.Models.Summarisation;
using ESFA.DC.Web.Operations.Utils;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.Web.Operations.Areas.PeriodEnd.Controllers
{
    [Area(AreaNames.PeriodEnd)]
    [Route(AreaNames.PeriodEnd + "/periodEndReports")]
    public class PeriodEndReportController : BaseControllerWithOpsPolicy
    {
        private const string CollectionType_ILR1920 = "ILR1920";
        private const string CollectionType_ESF = "ESF";
        private const string CollectionType_APPS = "APPS";
        private const int NumberofPreviousReturnPeriods = 2;

        private readonly IPeriodService _periodService;
        private readonly IPeriodEndService _periodEndService;
        private readonly IStorageService _storageService;
        private readonly ILogger _logger;

        public PeriodEndReportController(
            IPeriodService periodService,
            IPeriodEndService periodEndService,
            IStorageService storageService,
            ILogger logger,
            TelemetryClient telemetryClient)
            : base(logger, telemetryClient)
        {
            _logger = logger;
            _periodService = periodService;
            _periodEndService = periodEndService;
            _storageService = storageService;
        }

        [HttpGet("{collectionYear?}/{period?}")]
        public async Task<IActionResult> Index(int? collectionYear, int? period, CancellationToken cancellationToken)
        {
            var currentYearPeriod = await _periodService.ReturnPeriod();
            if (currentYearPeriod.Year == null)
            {
                throw new Exception($"Return period {currentYearPeriod.Period} has no year.");
            }

            var model = new PeriodEndReportViewModel();

            if (collectionYear != null && period != null)
            {
                model.Year = collectionYear.Value;
                model.Period = period.Value;
            }
            else
            {
                model.Year = currentYearPeriod.Year.Value;
                model.Period = currentYearPeriod.Period;
            }

            var reportDetails = _periodEndService.GetPeriodEndReportsAsync(model.Year, model.Period, cancellationToken);
            var sampleReports = _periodEndService.GetSampleReportsAsync(model.Year, model.Period, cancellationToken);
            var collectionStats = _periodEndService.GetCollectionStatsAsync(model.Year, model.Period, cancellationToken);
            var mcaReports = _periodEndService.GetMcaReportsAsync(model.Year, model.Period, cancellationToken);

            await Task.WhenAll(reportDetails, sampleReports, collectionStats, mcaReports);

            model.ReportDetails = reportDetails.Result;
            model.SampleReports = sampleReports.Result;
            model.CollectionStats = collectionStats.Result;
            model.McaReports = mcaReports.Result;

            model.SummarisationTotals = await GetSummarisationTotalsAsync(cancellationToken);

            return View(model);
        }

        [Route("getReportFile/{collectionYear}/{*fileName}")]
        public async Task<FileResult> GetReportFile(int collectionYear, string fileName, string downloadName = "")
        {
            try
            {
                var containerName = Utils.Constants.PeriodEndBlobContainerName.Replace(Utils.Constants.CollectionYearToken, collectionYear.ToString());
                var blobStream = await _storageService.GetFile(containerName, fileName, CancellationToken.None);

                return new FileStreamResult(blobStream, _storageService.GetMimeTypeFromFileName(fileName))
                {
                    FileDownloadName = string.IsNullOrEmpty(downloadName) ? fileName : downloadName
                };
            }
            catch (Exception e)
            {
                _logger.LogError($"Download report failed for file name : {fileName}", e);
                throw;
            }
        }

        [Route("getSampleReport/{collectionYear}/{period}/{*fileName}")]
        public async Task<FileResult> GetReportFile(int collectionYear, int period, string fileName)
        {
            var downloadName = $"{fileName.Substring(fileName.IndexOf('/') + 1, 8)}_{collectionYear}_R{period.ToString().PadLeft(2, '0')}_Reports.zip";

            return await GetReportFile(collectionYear, fileName, downloadName);
        }

        private async Task<IEnumerable<SummarisationTotal>> GetSummarisationTotalsAsync(CancellationToken cancellationToken)
        {
            var tasks = new List<Task<List<SummarisationCollectionReturnCode>>>();

            tasks.Add(_periodEndService.GetLatestSummarisationCollectionCodesAsync(CollectionType_ILR1920, NumberofPreviousReturnPeriods, cancellationToken));
            tasks.Add(_periodEndService.GetLatestSummarisationCollectionCodesAsync(CollectionType_ESF, NumberofPreviousReturnPeriods, cancellationToken));
            tasks.Add(_periodEndService.GetLatestSummarisationCollectionCodesAsync(CollectionType_APPS, NumberofPreviousReturnPeriods, cancellationToken));

            await Task.WhenAll(tasks);

            var summarisationReturnCodes = tasks.SelectMany(t => t.Result);

            var collectionReturnIds = summarisationReturnCodes.Select(x => x.Id).ToList();

            var summarisedData = await _periodEndService.GetSummarisationTotalsAsync(collectionReturnIds, cancellationToken);

            return summarisedData;
        }
    }
}