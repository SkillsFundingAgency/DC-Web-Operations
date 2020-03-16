﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.Jobs.Model.Enums;
using ESFA.DC.Jobs.Model.Processing.Detail;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Areas.Processing.Models;
using ESFA.DC.Web.Operations.Controllers;
using ESFA.DC.Web.Operations.Interfaces.Processing;
using ESFA.DC.Web.Operations.Utils;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.Web.Operations.Areas.Processing.Controllers
{
    [Area(AreaNames.Processing)]
    [Route(AreaNames.Processing + "/jobProcessingdetail")]
    public class JobProcessingDetailController : BaseController
    {
        private const string JobProcessingTypeCurrentPeriod = "CurrentPeriod";
        private const string JobProcessingTypeLastHour = "LastHour";
        private const string JobProcessingTypeLastFiveMins = "LastFiveMins";
        IJobProcessingDetailService _jobProcessingDetailService;
        ILogger _logger;
        private readonly IDateTimeProvider _dateTimeProvider;

        public JobProcessingDetailController(
            IJobProcessingDetailService jobProcessingDetailService,
            ILogger logger,
            IDateTimeProvider dateTimeProvider,
            TelemetryClient telemetryClient)
            : base(logger, telemetryClient)
        {
            _jobProcessingDetailService = jobProcessingDetailService;
            _logger = logger;
            _dateTimeProvider = dateTimeProvider;
        }

        [HttpGet("CurrentPeriod")]
        public async Task<IActionResult> CurrentPeriod(CancellationToken cancellationToken)
        {
            var jobDetails = await _jobProcessingDetailService.GetJobsProcessingDetailsForCurrentPeriod((short)JobStatusType.Completed, cancellationToken);
            var model = new JobProcessingDetailViewModel
            {
                Data = jobDetails.ToList(),
                JobProcessingType = JobProcessingTypeCurrentPeriod
            };
            return View("Index", model);
        }

        [HttpGet("LastHour")]
        public async Task<IActionResult> LastHour(CancellationToken cancellationToken)
        {
            var model = new JobProcessingDetailViewModel
            {
                Data = await GetJobsProcessingDetails(-60, cancellationToken),
                JobProcessingType = JobProcessingTypeLastHour
            };
            return View("Index", model);
        }

        [HttpGet("LastFiveMins")]
        public async Task<IActionResult> LastFiveMins(CancellationToken cancellationToken)
        {
            var model = new JobProcessingDetailViewModel
            {
                Data = await GetJobsProcessingDetails(-5, cancellationToken),
                JobProcessingType = JobProcessingTypeLastFiveMins
            };
            return View("Index", model);
        }

        private async Task<List<JobDetails>> GetJobsProcessingDetails(int minutes, CancellationToken cancellationToken)
        {
            var jobDetails = await _jobProcessingDetailService.GetJobsProcessingDetails(
                                                                (short)JobStatusType.Completed,
                                                                _dateTimeProvider.GetNowUtc().AddMinutes(minutes),
                                                                _dateTimeProvider.GetNowUtc(),
                                                                cancellationToken);
            return jobDetails.ToList();
        }
    }
}