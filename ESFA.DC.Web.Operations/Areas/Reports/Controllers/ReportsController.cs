using System;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Jobs.Model.Enums;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Areas.Reports.Models;
using ESFA.DC.Web.Operations.Interfaces.PeriodEnd;
using ESFA.DC.Web.Operations.Interfaces.Reports;
using ESFA.DC.Web.Operations.Interfaces.Storage;
using ESFA.DC.Web.Operations.Utils;
using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.Web.Operations.Areas.Reports.Controllers
{
    [Area(AreaNames.Reports)]
    [Route(AreaNames.Reports)]
    public class ReportsController : Controller
    {
        private readonly ILogger _logger;
        private readonly IStorageService _storageService;
        private readonly IPeriodService _periodService;
        private readonly IReportsService _reportsService;

        public ReportsController(
            ILogger logger,
            IStorageService storageService,
            IPeriodService periodService,
            IReportsService reportsService)
        {
            _logger = logger;
            _storageService = storageService;
            _periodService = periodService;
            _reportsService = reportsService;
        }

        [HttpGet("")]
        [HttpGet("Index")]
        public async Task<IActionResult> Index(int? collectionYear, int? collectionPeriod)
        {
            ViewBag.Error = TempData["error"];
            ReportsViewModel reportsViewModel = new ReportsViewModel();

            // validate parameters
            if (collectionYear.HasValue && collectionPeriod.HasValue && collectionPeriod.Value >= 1 && collectionPeriod.Value <= 14)
            {
                reportsViewModel.CollectionYear = collectionYear.Value;
                reportsViewModel.CollectionPeriod = collectionPeriod.Value;
            }
            else
            {
                // get the current period
                var currentYearPeriod = await _periodService.ReturnPeriod();

                if (currentYearPeriod == null)
                {
                    string errorMessage = $"Call to get current return period failed - collectionYear: {collectionYear} collectionPeriod: {collectionPeriod}";
                    _logger.LogError(errorMessage);
                    throw new Exception(errorMessage);
                }

                if (!currentYearPeriod.Year.HasValue)
                {
                    string errorMessage = $"Call to get current return period failed - collectionYear: {collectionYear} collectionPeriod: {collectionPeriod}";

                    _logger.LogError(errorMessage);
                    throw new Exception(errorMessage);
                }

                reportsViewModel.CollectionYear = currentYearPeriod.Year.Value;
                reportsViewModel.CollectionPeriod = currentYearPeriod.Period;
            }

            // get all the internal reports for the current period
            var reportDetails = await _reportsService.GetAllReportDetails(reportsViewModel.CollectionYear, reportsViewModel.CollectionPeriod);
            if (reportDetails != null)
            {
                reportsViewModel.ReportAction = ReportActions.GetReportDetails;
                reportsViewModel.Reports = reportDetails;
            }

            return View(model: reportsViewModel);
        }

        [HttpGet("RunReport/{reportType}/{collectionYear?}/{collectionPeriod?}")]
        public async Task<IActionResult> RunReport(string reportType, int? collectionYear, int? collectionPeriod)
        {
            // if collection period params not specified default to current period
            if (collectionYear == null || collectionPeriod == null)
            {
                var currentYearPeriod = await _periodService.ReturnPeriod();
                if (currentYearPeriod?.Year == null)
                {
                    string errorMessage = $"Call to get current return period failed in request {reportType} collectionYear: {collectionYear} collectionPeriod: {collectionPeriod}";
                    _logger.LogError(errorMessage);
                    throw new Exception(errorMessage);
                }

                collectionYear = currentYearPeriod.Year.Value;
                collectionPeriod = currentYearPeriod?.Period;
            }

            // queue the report job
            var jobId = await _reportsService.RunReport(reportType, collectionYear.Value, collectionPeriod.Value);

            // display the processing report spinner page while the report is running
            return RedirectToAction("ProcessingReport", "Reports", new { area = AreaNames.Reports, ReportType = reportType, ReportAction = ReportActions.ProcessingRunReport, CollectionYear = collectionYear, CollectionPeriod = collectionPeriod, JobId = jobId });
        }

        [HttpGet("GetReportFile/{collectionYear}/{fileName}/{downloadName?}")]
        public async Task<FileResult> GetReportFile(int collectionYear, string fileName, string downloadName = "")
        {
            try
            {
                fileName = fileName.Replace("%2F", "/");
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
