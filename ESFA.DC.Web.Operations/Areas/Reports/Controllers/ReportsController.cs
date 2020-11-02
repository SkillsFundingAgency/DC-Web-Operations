using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using ESFA.DC.Jobs.Model.Enums;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Areas.Reports.Models;
using ESFA.DC.Web.Operations.Constants;
using ESFA.DC.Web.Operations.Controllers;
using ESFA.DC.Web.Operations.Extensions;
using ESFA.DC.Web.Operations.Interfaces.Collections;
using ESFA.DC.Web.Operations.Interfaces.PeriodEnd;
using ESFA.DC.Web.Operations.Interfaces.Reports;
using ESFA.DC.Web.Operations.Interfaces.Storage;
using ESFA.DC.Web.Operations.Models.Reports;
using ESFA.DC.Web.Operations.Utils;
using ESFA.DC.Web.Operations.Utils.Extensions;
using Microsoft.ApplicationInsights;
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
        private readonly IEnumerable<IReport> _reports;
        private readonly IReportsService _reportsService;

        public ReportsController(
            ILogger logger,
            IStorageService storageService,
            IPeriodService periodService,
            IReportsService reportsService,
            ICollectionsService collectionsService,
            IEnumerable<IReport> Reports,
            TelemetryClient telemetryClient)
            : base(logger, telemetryClient)
        {
            _logger = logger;
            _storageService = storageService;
            _periodService = periodService;
            _reportsService = reportsService;
            _collectionsService = collectionsService;
            _reports = Reports;
        }

        [HttpGet("")]
        [HttpGet("Index")]
        public async Task<IActionResult> Index(ReportsViewModel viewModel, CancellationToken cancellationToken)
        {
            var model = new ReportsViewModel();
            ViewBag.Error = TempData["error"];

            if (viewModel != null && viewModel.CollectionYear != 0)
            {
                model = await GenerateReportsViewModel(viewModel, cancellationToken);
            }
            else
            {
                var currentYearPeriod = await _periodService.ReturnPeriod(CollectionTypes.ILR, cancellationToken);
                model.CollectionYear = currentYearPeriod.Year.GetValueOrDefault();
                model.CollectionPeriod = currentYearPeriod.Period;
                model = await GenerateReportsViewModel(model, cancellationToken);
            }

            model.ValidationReportGenerationUrl = Url.Action(nameof(RuleValidationController.ValidationRulesReport), "RuleValidation");
            model.ReportsUrl = Url.Action(nameof(ReportsController.Index), "Reports");
            model.ReportGenerationUrl = Url.Action(nameof(ReportsController.RunReport), "Reports");
            model.ReportsDownloadUrl = Url.Action(nameof(ReportsController.GetReportFile), "Reports");

            return View(model);
        }

        [HttpGet("RunReport")]
        public async Task<IActionResult> RunReport(string reportName, int? year, int? period)
        {
            // if collection period params not specified default to current period
            if (year == null || period == null)
            {
                var currentYearPeriod = await _periodService.GetRecentlyClosedPeriodAsync();
                if (currentYearPeriod?.CollectionYear == null)
                {
                    string errorMessage = $"Call to get current return period failed in request {reportName} collectionYear: {year} collectionPeriod: {period}";
                    _logger.LogError(errorMessage);
                    throw new InvalidOperationException(errorMessage);
                }

                year = currentYearPeriod.CollectionYear;
                period = currentYearPeriod.PeriodNumber;
            }

            // queue the report job
            var jobId = await _reportsService.RunReport(reportName, year.Value, period.Value, User.Name());

            // display the processing report spinner page while the report is running
            return RedirectToAction("ProcessingReport", "Reports", new { ReportName = reportName, ReportAction = ReportActions.ProcessingRunReport, CollectionYear = year, CollectionPeriod = period, JobId = jobId });
        }

        [HttpGet("GetReportFile")]
        public async Task<FileResult> GetReportFile(int collectionYear, int collectionPeriod, string fileName, string reportDisplayName,  string downloadName = "")
        {
            try
            {
                var periodString = $"R{collectionPeriod:D2}";
                var decodedFileName = HttpUtility.HtmlDecode(fileName);
                var report = _reports.Single(x => x.DisplayName.CaseInsensitiveEquals(reportDisplayName));
                var containerName = report.ContainerName.Replace(Utils.Constants.CollectionYearToken, collectionYear.ToString());
                fileName = _reportsService.BuildFileName(report.ReportType, collectionYear, periodString, decodedFileName);
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

        [HttpGet("ProcessingReport/{reportName}/{collectionYear}/{collectionPeriod}/{jobId?}/{jobStatusType?}")]
        public async Task<IActionResult> ProcessingReport(string reportName, int collectionYear, int collectionPeriod, long? jobId, JobStatusType? jobStatusType)
        {
            ViewBag.AutoRefresh = true;
            ReportsViewModel reportViewModel = new ReportsViewModel()
            {
                ReportName = reportName,
                CollectionYear = collectionYear,
                CollectionPeriod = collectionPeriod,
                JobId = jobId
            };

            var jobStatus = (JobStatusType)await _reportsService.GetReportStatus(jobId);
            if (jobStatus == JobStatusType.Failed || jobStatus == JobStatusType.FailedRetry)
            {
                _logger.LogError($"Loading in progress page for job id : {jobId}, job is in status ; {jobStatus} - user will be sent to service error page");
                ModelState.AddModelError(ErrorMessageKeys.ErrorSummaryKey, $"Report generation for - {reportName}, with JobId: {jobId} has failed");
                return View(model: reportViewModel);
            }

            if (jobStatus != JobStatusType.Completed)
            {
                return View(model: reportViewModel);
            }

            return RedirectToAction("Index", "Reports", reportViewModel);
        }

        private async Task<ReportsViewModel> GenerateReportsViewModel(ReportsViewModel reportsViewModel, CancellationToken cancellationToken)
        {
            var model = new ReportsViewModel()
            {
                CollectionYear = reportsViewModel.CollectionYear,
                CollectionPeriod = reportsViewModel.CollectionPeriod,
            };

            var getAllPeriodsTask = _periodService.GetPeriodsUptoNowAsync(CollectionTypes.ILR, cancellationToken);
            var collectionYearsTask = _collectionsService.GetCollectionYearsByType(CollectionTypes.ILR, cancellationToken);

            await Task.WhenAll(getAllPeriodsTask, collectionYearsTask);

            model.ReturnPeriods = getAllPeriodsTask.Result;
            model.CollectionYears = collectionYearsTask.Result;

            return model;
        }
    }
}
