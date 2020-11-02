using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Features.Indexed;
using ESFA.DC.FileService.Interface;
using ESFA.DC.Jobs.Model;
using ESFA.DC.Jobs.Model.Enums;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Operations.Reports.Model;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Web.Operations.Areas.Reports.Models;
using ESFA.DC.Web.Operations.Constants;
using ESFA.DC.Web.Operations.Controllers;
using ESFA.DC.Web.Operations.Extensions;
using ESFA.DC.Web.Operations.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.Collections;
using ESFA.DC.Web.Operations.Interfaces.Storage;
using ESFA.DC.Web.Operations.Interfaces.ValidationRules;
using ESFA.DC.Web.Operations.Utils;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ESFA.DC.Web.Operations.Areas.Reports.Controllers
{
    [Area(AreaNames.Reports)]
    [Route(AreaNames.Reports)]
    public class RuleValidationController : BaseControllerWithDevOpsOrAdvancedSupportOrReportsPolicy
    {
        private readonly string validationRuleDetailsReportJson = "Reports/{0}/Validation Rule Details.json";
        private readonly string validationRuleDetailsReportCsv = "Reports/{0}/Validation Rule Details.csv";
        private readonly ILogger _logger;
        private readonly ICollectionsService _collectionsService;
        private readonly IValidationRulesService _validationRulesService;
        private readonly IJobService _jobService;
        private readonly IStorageService _storageService;
        private readonly IJsonSerializationService _jsonSerializationService;
        private readonly IFileService _operationsFileService;

        public RuleValidationController(
            ILogger logger,
            ICollectionsService collectionsService,
            IValidationRulesService validationRulesService,
            IJobService jobService,
            IStorageService storageService,
            IJsonSerializationService jsonSerializationService,
            IIndex<PersistenceStorageKeys, IFileService> operationsFileService,
            TelemetryClient telemetryClient)
            : base(logger, telemetryClient)
        {
            _logger = logger;
            _collectionsService = collectionsService;
            _validationRulesService = validationRulesService;
            _jobService = jobService;
            _storageService = storageService;
            _jsonSerializationService = jsonSerializationService;
            _operationsFileService = operationsFileService[PersistenceStorageKeys.OperationsAzureStorage];
        }

        [HttpGet("ValidationRulesReport")]
        public async Task<IActionResult> ValidationRulesReport(int year, int period, string rule)
        {
            var model = new RuleSearchViewModel();
            var collectionYears = await _collectionsService.GetCollectionYearsByType(CollectionTypeConstants.Ilr);
            model.CollectionYears = collectionYears.OrderByDescending(x => x).ToList();
            ViewData["years"] = model.CollectionYears.Select(x => new SelectListItem { Text = x.ToString(), Value = x.ToString() }).ToList();
            var jobId = await _validationRulesService.GenerateReport(rule, year, User.Name());
            return RedirectToAction("InProgress", new { jobId, period });
        }

        [HttpGet("InProgress")]
        public async Task<IActionResult> InProgress(long jobId, int period)
        {
            ViewBag.AutoRefresh = true;
            var jobStatus = await _jobService.GetJobStatus(jobId);
            if (jobStatus == JobStatusType.Failed || jobStatus == JobStatusType.FailedRetry)
            {
                _logger.LogError($"Loading in progress page for job id : {jobId}, job is in status ; {jobStatus} - user will be sent to service error page");
                ModelState.AddModelError(ErrorMessageKeys.ErrorSummaryKey, $"Job {jobId} has failed");
                return View();
            }

            if (jobStatus != JobStatusType.Completed)
            {
                return View();
            }

            return RedirectToAction("Report", new { jobId, period });
        }

        [HttpGet("Report")]
        public async Task<IActionResult> Report(long jobId, int period)
        {
            var model = new ValidationRuleDetailReportViewModel();
            List<ValidationRuleDetail> validationRuleDetails = new List<ValidationRuleDetail>();
            var validationRuleDetailsReportJsonFile = string.Format(validationRuleDetailsReportJson, jobId);
            var job = await _jobService.GetJob(0, jobId);
            model.JobId = job.JobId;
            model.ContainerName = job.StorageReference;
            model.ReportFileName = string.Format(validationRuleDetailsReportCsv, jobId);
            model.Rule = job.Rule;
            model.Year = job.SelectedCollectionYear;
            model.Period = period;
            model.ValidationReportGenerationUrl = Url.Action(nameof(RuleValidationController.ValidationRulesReport), "RuleValidation");

            // json file
            using (var stream = await _operationsFileService.OpenReadStreamAsync(validationRuleDetailsReportJsonFile, job.StorageReference, CancellationToken.None))
            {
                validationRuleDetails = _jsonSerializationService.Deserialize<List<ValidationRuleDetail>>(stream);
            }

            model.ValidationRuleDetailsByReturnPeriod = validationRuleDetails.GroupBy(x => x.ReturnPeriod).ToDictionary(group => group.Key, group => group.ToList());
            return View(model);
        }

        [HttpPost("DownloadReport")]
        public async Task<IActionResult> DownloadReport(string containerName, string reportFilename, string rule, int year, int period, int jobId)
        {
            var blobStream = await _operationsFileService.OpenReadStreamAsync(reportFilename, containerName, CancellationToken.None);

            var periodString = $"R{period.ToString("00", NumberFormatInfo.InvariantInfo)}";

            return new FileStreamResult(blobStream, _storageService.GetMimeTypeFromFileName(reportFilename))
            {
                FileDownloadName = $"{rule}_{year}_{periodString}_ValidationRuleDetailReport.csv"
            };
        }
    }
}
