using System;
using System.Collections.Generic;
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
using ESFA.DC.Web.Operations.Models.Reports;
using ESFA.DC.Web.Operations.Utils;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

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
        private readonly IEnumerable<IReport> _reports;
        private readonly IAuthorizationService _authorizationService;

        public ReportsController(
            ILogger logger,
            IStorageService storageService,
            IPeriodService periodService,
            IReportsService reportsService,
            ICollectionsService collectionsService,
            TelemetryClient telemetryClient,
            IEnumerable<IReport> reports,
            IAuthorizationService authorizationService)
            : base(logger, telemetryClient)
        {
            _logger = logger;
            _storageService = storageService;
            _periodService = periodService;
            _reportsService = reportsService;
            _collectionsService = collectionsService;
            _reports = reports;
            _authorizationService = authorizationService;
        }

        [HttpGet("")]
        [HttpGet("Index")]
        [HttpPost("Index")]
        public async Task<IActionResult> Index(ReportsViewModel viewModel, CancellationToken cancellationToken)
        {
            var model = new ReportsViewModel();

            ViewBag.Error = TempData["error"];

            var authorisedReports = new List<IReport>();

            foreach (var report in _reports)
            {
                if (await IsAuthorised(report))
                {
                    authorisedReports.Add(report);
                }
            }

            if (viewModel != null && viewModel.CollectionYear != 0)
            {
                model = await GenerateReportsViewModel(viewModel, authorisedReports, cancellationToken);
            }
            else
            {
                var currentYearPeriod = await _periodService.ReturnPeriod(CollectionTypes.ILR, cancellationToken);
                model.CurrentCollectionYear = currentYearPeriod.Year.GetValueOrDefault();
                model.CurrentCollectionPeriod = currentYearPeriod.Period;
                model.CollectionYear = currentYearPeriod.Year.GetValueOrDefault();
                model.CollectionPeriod = currentYearPeriod.Period;
                model = await GenerateReportsViewModel(model, authorisedReports, cancellationToken);
            }

            return View(model);
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
                period = currentYearPeriod.PeriodNumber;
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
                    }

                    break;

                case ReportActions.RunReport:
                    // Create a report job
                    reportViewModel.JobId = await _reportsService.RunReport(reportType, collectionYear, collectionPeriod);
                    break;

                case ReportActions.GetReportDetails:
                    // redirect to the index page to display all the reports
                    return RedirectToAction("Index", "Reports", new { area = AreaNames.Reports, CollectionYear = collectionYear, CollectionPeriod = collectionPeriod });
            }

            return View(model: reportViewModel);
        }

        private async Task<bool> IsAuthorised(IReport report)
        {
            return (await _authorizationService.AuthorizeAsync(User, report.Policy)).Succeeded;
        }

        private async Task<ReportsViewModel> GenerateReportsViewModel(ReportsViewModel reportsViewModel, IEnumerable<IReport> authorisedReports, CancellationToken cancellationToken)
        {
            var model = new ReportsViewModel()
            {
                CurrentCollectionPeriod = reportsViewModel.CurrentCollectionPeriod,
                CurrentCollectionYear = reportsViewModel.CurrentCollectionYear,
                CollectionYear = reportsViewModel.CollectionYear,
                CollectionPeriod = reportsViewModel.CollectionPeriod,
                ReportAction = ReportActions.GetReportDetails
            };

            var getAllPeriodsTask = _periodService.GetAllPeriodsAsync(CollectionTypes.ILR, cancellationToken);
            var collectionYearsTask = _collectionsService.GetCollectionYearsByType(CollectionTypes.ILR, cancellationToken);
            var reportsTask = _reportsService.GetAvailableReportsAsync(model.CurrentCollectionYear, authorisedReports, cancellationToken);

            await Task.WhenAll(getAllPeriodsTask, collectionYearsTask, reportsTask);

            model.ReportPeriods = await getAllPeriodsTask;
            model.CollectionYears = await collectionYearsTask;
            model.Reports = (await reportsTask).Select(x => new SelectListItem(x.DisplayName, x.ReportName));

            return model;
        }
    }
}
