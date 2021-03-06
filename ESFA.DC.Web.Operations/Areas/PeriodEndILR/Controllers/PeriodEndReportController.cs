﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Areas.PeriodEndILR.Models;
using ESFA.DC.Web.Operations.Controllers;
using ESFA.DC.Web.Operations.Interfaces.PeriodEnd;
using ESFA.DC.Web.Operations.Interfaces.Storage;
using ESFA.DC.Web.Operations.Models.Summarisation;
using ESFA.DC.Web.Operations.Utils;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.Web.Operations.Areas.PeriodEndILR.Controllers
{
    [Area(AreaNames.PeriodEndILR)]
    [Route(AreaNames.PeriodEndILR + "/periodEndReports")]
    public class PeriodEndReportController : BaseControllerWithDevOpsOrAdvancedSupportPolicy
    {
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
            var currentYearPeriod = await _periodService.ReturnPeriod(CollectionTypes.ILR, cancellationToken);
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

            var reportDetailsTask = _periodEndService.GetPeriodEndReportsAsync(model.Year, model.Period, cancellationToken);
            var sampleReportsTask = _periodEndService.GetSampleReportsAsync(model.Year, model.Period, cancellationToken);
            var llvSampleReportsTask = _periodEndService.GetLLVSampleReportsAsync(model.Year, model.Period, cancellationToken);
            var collectionStatsTask = _periodEndService.GetCollectionStatsAsync(model.Year, model.Period, cancellationToken);
            var mcaReportsTask = _periodEndService.GetMcaReportsAsync(model.Year, model.Period, cancellationToken);

            await Task.WhenAll(reportDetailsTask, sampleReportsTask, llvSampleReportsTask, collectionStatsTask, mcaReportsTask);

            model.ReportDetails = reportDetailsTask.Result;
            model.SampleReports = sampleReportsTask.Result;
            model.LLVSampleReports = llvSampleReportsTask.Result;
            model.CollectionStats = collectionStatsTask.Result;
            model.McaReports = mcaReportsTask.Result;

            model.SummarisationTotals = await GetSummarisationTotalsAsync(model.Year, model.Period, cancellationToken);

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

        [Route("getLLVSampleReport/{collectionYear}/{period}/{*fileName}")]
        public async Task<FileResult> GetLLVReportFile(int collectionYear, int period, string fileName)
        {
            var file = fileName.Substring(fileName.LastIndexOf("/") + 1);
            var downloadName = $"{fileName.Substring(fileName.IndexOf('/') + 1, 8)}_{collectionYear}_R{period.ToString().PadLeft(2, '0')}_{file}";

            return await GetReportFile(collectionYear, fileName, downloadName);
        }

        [Route("getSampleReport/{collectionYear}/{period}/{*fileName}")]
        public async Task<FileResult> GetReportFile(int collectionYear, int period, string fileName)
        {
            var downloadName = $"{fileName.Substring(fileName.IndexOf('/') + 1, 8)}_{collectionYear}_R{period.ToString().PadLeft(2, '0')}_Reports.zip";

            return await GetReportFile(collectionYear, fileName, downloadName);
        }

        private async Task<IEnumerable<SummarisationTotal>> GetSummarisationTotalsAsync(int collectionYear, int period, CancellationToken cancellationToken)
        {
            var tasks = new List<Task<List<SummarisationCollectionReturnCode>>>();

            tasks.Add(_periodEndService.GetSummarisationCollectionCodesAsync(CollectionTypes.ILR, collectionYear, period, cancellationToken));
            tasks.Add(_periodEndService.GetSummarisationCollectionCodesAsync(CollectionTypes.ESF, collectionYear, period, cancellationToken));
            tasks.Add(_periodEndService.GetSummarisationCollectionCodesAsync(CollectionTypes.APPS, collectionYear, period, cancellationToken));

            await Task.WhenAll(tasks);

            var summarisationReturnCodes = tasks.SelectMany(t => t.Result);

            var collectionReturnIds = summarisationReturnCodes.Select(x => x.Id).ToList();

            var summarisedData = await _periodEndService.GetSummarisationTotalsAsync(collectionReturnIds, cancellationToken);

            return summarisedData;
        }
    }
}