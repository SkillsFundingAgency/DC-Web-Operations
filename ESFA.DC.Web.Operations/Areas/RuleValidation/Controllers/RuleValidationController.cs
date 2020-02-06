﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Features.AttributeFilters;
using Autofac.Features.Indexed;
using ESFA.DC.FileService.Interface;
using ESFA.DC.Jobs.Model;
using ESFA.DC.Jobs.Model.Enums;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Web.Operations.Areas.RuleValidation.Models;
using ESFA.DC.Web.Operations.Extensions;
using ESFA.DC.Web.Operations.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.Collections;
using ESFA.DC.Web.Operations.Interfaces.Storage;
using ESFA.DC.Web.Operations.Interfaces.ValidationRules;
using ESFA.DC.Web.Operations.Settings.Models;
using ESFA.DC.Web.Operations.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ESFA.DC.Web.Operations.Areas.RuleValidation.Controllers
{
    [Area(AreaNames.RuleValidation)]
    [Route(AreaNames.RuleValidation)]
    public class RulevalidationController : Controller
    {
        private readonly string validationRuleDetailsReportJson = "Reports/{0}/Validation Rule Details.json";
        private readonly string validationRuleDetailsReportCsv = "Reports/{0}/Validation Rule Details.csv";
        private readonly ILogger _logger;
        private readonly ICollectionsService _collectionsService;
        private readonly IValidationRulesService _validationRulesService;
        private readonly IJobService _jobService;
        private readonly IStorageService _storageService;
        private readonly OpsDataLoadServiceConfigSettings _opsDataLoadServiceConfigSettings;
        private readonly IJsonSerializationService _jsonSerializationService;
        private readonly IFileService _operationsFileService;

        public RulevalidationController(
            ILogger logger,
            ICollectionsService collectionsService,
            IValidationRulesService validationRulesService,
            IJobService jobService,
            IStorageService storageService,
            OpsDataLoadServiceConfigSettings opsDataLoadServiceConfigSettings,
            IJsonSerializationService jsonSerializationService,
            IIndex<PersistenceStorageKeys, IFileService> operationsFileService)
        {
            _logger = logger;
            _collectionsService = collectionsService;
            _validationRulesService = validationRulesService;
            _jobService = jobService;
            _storageService = storageService;
            _opsDataLoadServiceConfigSettings = opsDataLoadServiceConfigSettings;
            _jsonSerializationService = jsonSerializationService;
            _operationsFileService = operationsFileService[PersistenceStorageKeys.OperationsAzureStorage];
        }

        [HttpGet("")]
        [HttpGet("Index")]
        public async Task<IActionResult> Index()
        {
            var model = new RuleSearchViewModel();
            var collectionYears = await _collectionsService.GetCollectionYearsByType(CollectionTypeConstants.Ilr);
            model.CollectionYears = collectionYears.OrderByDescending(x => x).ToList();
            model.Rules = await _validationRulesService.GetValidationRules(model.CollectionYears.ElementAt(0));
            ViewData["years"] = model.CollectionYears.Select(x => new SelectListItem { Text = x.ToString(), Value = x.ToString() }).ToList();
            return View(model);
        }

        [HttpPost("")]
        [HttpPost("Index")]
        public async Task<IActionResult> Index(int year)
        {
            var model = new RuleSearchViewModel();
            var collectionYears = await _collectionsService.GetCollectionYearsByType(CollectionTypeConstants.Ilr);
            model.CollectionYears = collectionYears.OrderByDescending(x => x).ToList();
            model.Rules = await _validationRulesService.GetValidationRules(year);
            ViewData["years"] = model.CollectionYears.Select(x => new SelectListItem { Text = x.ToString(), Value = x.ToString() }).ToList();
            return View(model);
        }

        [HttpGet("ValidationRulesReport")]
        public async Task<IActionResult> ValidationRulesReport(int year, string rule)
        {
            var model = new RuleSearchViewModel();
            var collectionYears = await _collectionsService.GetCollectionYearsByType(CollectionTypeConstants.Ilr);
            model.CollectionYears = collectionYears.OrderByDescending(x => x).ToList();
            model.Rules = await _validationRulesService.GetValidationRules(year);
            ViewData["years"] = model.CollectionYears.Select(x => new SelectListItem { Text = x.ToString(), Value = x.ToString() }).ToList();

            var jobId = await _validationRulesService.GenerateReport(rule, year, User.Name());

            return RedirectToAction("InProgress", new { jobId });
        }

        [HttpGet("InProgress")]
        public async Task<IActionResult> InProgress(long jobId)
        {
            ViewBag.AutoRefresh = true;

            var jobStatus = await _jobService.GetJobStatus(jobId);

            if (jobStatus == JobStatusType.Failed || jobStatus == JobStatusType.FailedRetry)
            {
                _logger.LogError($"Loading in progress page for job id : {jobId}, job is in status ; {jobStatus} - user will be sent to service error page");
                TempData["JobFailed"] = $"Job {jobId} has failed";
                return RedirectToAction("Index");
            }

            if (jobStatus != JobStatusType.Completed)
            {
                return View();
            }

            return RedirectToAction("Report", new { jobId });
        }

        [HttpGet("Report")]
        public async Task<IActionResult> Report(long jobId)
        {
            var model = new ReportViewModel();
            List<ValidationRuleDetail> validationRuleDetails;
            var validationRuleDetailsReportJsonFile = string.Format(validationRuleDetailsReportJson, jobId);
            var job = await _jobService.GetJob(0, jobId);
            model.JobId = job.JobId;
            model.ContainerName = job.StorageReference;
            model.ReportFileName = string.Format(validationRuleDetailsReportCsv, jobId);

            //todo: get the validationruledetailsreport job

            model.Rule = "abc"; //job.Rule;
            model.Year = 1920; //job.SelectedCollectionYear;
            //// json file
             using (var stream = await _operationsFileService.OpenReadStreamAsync(validationRuleDetailsReportJsonFile, job.StorageReference, CancellationToken.None))
            {
                validationRuleDetails = _jsonSerializationService.Deserialize<List<ValidationRuleDetail>>(stream);
            }

            var persistenceService = await _storageService.GetAzureStorageReferenceService(_opsDataLoadServiceConfigSettings.ConnectionString, job.StorageReference);
            var summaryExists = await persistenceService.ContainsAsync(validationRuleDetailsReportJsonFile);
            if (summaryExists)
            {
                var data = await persistenceService.GetAsync(validationRuleDetailsReportJsonFile);
//                var validationRuleDetails = _jsonSerializationService.Deserialize<List<ValidationRuleDetail>>(data);

                if (validationRuleDetails.Any())
                {
                    model.Validationruledetails = new Dictionary<string, List<ValidationRuleDetail>>();
                    var groupBy = validationRuleDetails.GroupBy(x => x.ReturnPeriod);
                    model.Validationruledetails = validationRuleDetails.GroupBy(x => x.ReturnPeriod).ToDictionary(group => group.Key, group => group.ToList());
                }
            }

            return View(model);
        }

        [HttpPost("DownloadReport")]
        public async Task<IActionResult> DownloadReport(string containerName, string reportFilename, string rule, int year, int jobId)
        {
            using (var stream = await _operationsFileService.OpenReadStreamAsync(reportFilename, containerName, CancellationToken.None))
             {
                return new FileStreamResult(stream, _storageService.GetMimeTypeFromFileName(reportFilename))
                 {
                     FileDownloadName = $"ValidationRuleDetailReport_{rule}_{year}.csv"
                 };
            }
        }
    }
}
