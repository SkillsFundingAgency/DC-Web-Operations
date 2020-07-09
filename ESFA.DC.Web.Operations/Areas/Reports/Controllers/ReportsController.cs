using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using ESFA.DC.Jobs.Model.Enums;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Areas.Reports.Models;
using ESFA.DC.Web.Operations.Controllers;
using ESFA.DC.Web.Operations.Interfaces.Collections;
using ESFA.DC.Web.Operations.Interfaces.PeriodEnd;
using ESFA.DC.Web.Operations.Interfaces.Reports;
using ESFA.DC.Web.Operations.Interfaces.Storage;
using ESFA.DC.Web.Operations.Utils;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.Web.Operations.Areas.Reports.Controllers
{
    [Area(AreaNames.Reports)]
    [Route(AreaNames.Reports)]
    public class ReportsController : BaseControllerWithDevOpsOrAdvancedSupportOrReportsPolicy
    {
        private readonly ILogger _logger;
        private readonly IStorageService _storageService;
        private readonly IPeriodService _periodService;
        private readonly ICollectionsService _collectionsService;
        private readonly IReportsService _reportsService;

        public ReportsController(
            ILogger logger,
            IStorageService storageService,
            IPeriodService periodService,
            IReportsService reportsService,
            ICollectionsService collectionsService,
            TelemetryClient telemetryClient)
            : base(logger, telemetryClient)
        {
            _logger = logger;
            _storageService = storageService;
            _periodService = periodService;
            _reportsService = reportsService;
            _collectionsService = collectionsService;
        }

        [HttpGet("")]
        [HttpGet("Index")]
        public async Task<IActionResult> Index(int? collectionYear, int? collectionPeriod, CancellationToken cancellationToken)
        {
            const string IlrCollectionType = "ILR";

            ViewBag.Error = TempData["error"];
            ReportsViewModel reportsViewModel = new ReportsViewModel()
            {
                ReportPeriods = await _periodService.GetAllPeriodsAsync(IlrCollectionType, cancellationToken),
                CollectionYears = await _collectionsService.GetCollectionYearsByType(IlrCollectionType, cancellationToken)
            };

            // get the current period
            var currentYearPeriod = await _periodService.ReturnPeriod(CollectionTypes.ILR, cancellationToken);

            reportsViewModel.CurrentCollectionYear = currentYearPeriod.Year ?? 0;
            reportsViewModel.CurrentCollectionPeriod = currentYearPeriod.Period;

            // validate parameters
            if (collectionYear.HasValue && collectionPeriod.HasValue && collectionPeriod.Value >= 1 && collectionPeriod.Value <= 14)
            {
                reportsViewModel.CollectionYear = collectionYear.Value;
                reportsViewModel.CollectionPeriod = collectionPeriod.Value;
            }
            else
            {
                reportsViewModel.CollectionYear = currentYearPeriod.Year ?? 0;
                reportsViewModel.CollectionPeriod = currentYearPeriod.Period;
            }

            reportsViewModel.ReportAction = ReportActions.GetReportDetails;

            return View(reportsViewModel);
        }

        [HttpGet("RunReport")]
        public async Task<IActionResult> RunReport(string reportType, int? year, int? period)
        {
            // if collection period params not specified default to current period
            if (year == null || period == null)
            {
                var currentYearPeriod = await _periodService.GetRecentlyClosedPeriodAsync();
                if (currentYearPeriod?.CollectionYear == null)
                {
                    string errorMessage = $"Call to get current return period failed in request {reportType} collectionYear: {year} collectionPeriod: {period}";
                    _logger.LogError(errorMessage);
                    throw new InvalidOperationException(errorMessage);
                }

                year = currentYearPeriod.CollectionYear;
                period = currentYearPeriod?.PeriodNumber;
            }

            // queue the report job
            var jobId = await _reportsService.RunReport(reportType, year.Value, period.Value);

            // display the processing report spinner page while the report is running
            return RedirectToAction("ProcessingReport", "Reports", new { ReportType = reportType, ReportAction = ReportActions.ProcessingRunReport, CollectionYear = year, CollectionPeriod = period, JobId = jobId });
        }

        [HttpGet("GetReportFile")]
        public async Task<FileResult> GetReportFile(int collectionYear, int collectionPeriod, string fileName, string downloadName = "")
        {
            try
            {
                var containerName = Utils.Constants.ReportsBlobContainerName.Replace(Utils.Constants.CollectionYearToken, collectionYear.ToString());

                fileName = $"R{collectionPeriod.ToString("D2")}/{HttpUtility.HtmlDecode(fileName)}";

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

        [HttpGet("ProcessingReport/{reportType}/{reportAction}/{collectionYear}/{collectionPeriod}/{jobId?}/{jobStatusType?}")]
        public async Task<IActionResult> ProcessingReport(string reportType, string reportAction, int collectionYear, int collectionPeriod, long? jobId, JobStatusType? jobStatusType)
        {
            if (string.IsNullOrEmpty(reportAction))
            {
                string errorMessage = $"Missing 'reportAction' parameter for 'reportAction' in request {reportType} collectionYear: {collectionYear} collectionPeriod: {collectionPeriod}";
                _logger.LogError(errorMessage);
                throw new Exception(errorMessage);
            }

            ReportsViewModel reportViewModel = new ReportsViewModel()
            {
                ReportType = reportType,
                ReportAction = reportAction,
                CollectionYear = collectionYear,
                CollectionPeriod = collectionPeriod,
                JobId = jobId
            };

            switch (reportAction)
            {
                case ReportActions.ProcessingRunReport:
                    // get the job status
                    reportViewModel.ReportStatus = (JobStatusType)await _reportsService.GetReportStatus(jobId);

                    switch (reportViewModel.ReportStatus)
                    {
                        case JobStatusType.Failed:
                        case JobStatusType.FailedRetry:
                            string errorMessage = $"The report status was '{reportViewModel.ReportStatus}' for job '{jobId}'";
                            _logger.LogError(errorMessage);
                            TempData["Error"] = errorMessage;

                            // if job has failed or failed retry display the error on the index page
                            return RedirectToAction("Index", "Reports", new { area = AreaNames.Reports, CollectionYear = collectionYear, CollectionPeriod = collectionPeriod });

                        case JobStatusType.Completed:
                            // if job has completed, redirect to Index page to show all the reports
                            return RedirectToAction("Index", "Reports", new { area = AreaNames.Reports, CollectionYear = collectionYear, CollectionPeriod = collectionPeriod });

                        default:
                            break;
                    }

                    break;

                case ReportActions.RunReport:
                    // Create a report job
                    reportViewModel.JobId = await _reportsService.RunReport(reportType, collectionYear, collectionPeriod);
                    break;

                case ReportActions.GetReportDetails:
                    // redirect to the index page to display all the reports
                    return RedirectToAction("Index", "Reports", new { area = AreaNames.Reports, CollectionYear = collectionYear, CollectionPeriod = collectionPeriod });

                default:
                    break;
            }

            return View(model: reportViewModel);
        }
    }
}
