﻿using System;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.PeriodEnd;
using ESFA.DC.Web.Operations.Interfaces.Storage;
using ESFA.DC.Web.Operations.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.Web.Operations.Controllers
{
    [Route("periodEndReports")]
    public class PeriodEndReportController : Controller
    {
        private readonly IPeriodService _periodService;
        private readonly IPeriodEndService _periodEndService;
        private readonly IStorageService _storageService;
        private readonly ILogger _logger;

        public PeriodEndReportController(
            IPeriodService periodService,
            IPeriodEndService periodEndService,
            IStorageService storageService,
            ILogger logger)
        {
            _periodService = periodService;
            _periodEndService = periodEndService;
            _storageService = storageService;
            _logger = logger;
        }

        [HttpGet("{collectionYear?}/{period?}")]
        public async Task<IActionResult> Index(int? collectionYear, int? period)
        {
            var currentYearPeriod = await _periodService.ReturnPeriod();
            var model = new PeriodEndReportViewModel();

            if (collectionYear != null && period != null)
            {
                model.Year = collectionYear.Value;
                model.Period = period.Value;
            }
            else
            {
                model.Year = currentYearPeriod.Year;
                model.Period = currentYearPeriod.Period;
            }

            var reportDetails = await _periodEndService.GetPeriodEndReports(model.Year, model.Period);

            var sampleReports = await _periodEndService.GetSampleReports(model.Year, model.Period, CancellationToken.None);

            model.ReportDetails = reportDetails;
            model.SampleReports = sampleReports;

            return View(model);
        }

        [Route("getReportFile/{collectionYear}/{*fileName}")]
        public async Task<FileResult> GetReportFile(int collectionYear, string fileName, string downloadName = "")
        {
            try
            {
                var blobStream = await _storageService.GetFile(collectionYear, fileName, CancellationToken.None);

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
            var downloadName = $"{fileName.Substring(0, fileName.IndexOf('/'))}_R{period.ToString().PadLeft(2, '0')}_Reports.zip";

            return await GetReportFile(collectionYear, fileName, downloadName);
        }
    }
}