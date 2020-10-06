using System;
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
        private readonly IJobProcessingDetailService _jobProcessingDetailService;
        private readonly ILogger _logger;
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
            var model = await BuildJobProcessingDetailModel(-60, JobProcessingTypeLastHour, cancellationToken);
            return View("Index", model);
        }

        [HttpGet("LastFiveMins")]
        public async Task<IActionResult> LastFiveMins(CancellationToken cancellationToken)
        {
            var model = await BuildJobProcessingDetailModel(-5, JobProcessingTypeLastFiveMins, cancellationToken);
            return View("Index", model);
        }

        public IActionResult IlrReturnsCurrentPeriod()
        {
            throw new NotImplementedException();
        }

        private async Task<JobProcessingDetailViewModel> BuildJobProcessingDetailModel(int minutes, string jobProcessingType, CancellationToken cancellationToken)
        {
            var dateTimeUtc = _dateTimeProvider.GetNowUtc();
            var jobDetails = await _jobProcessingDetailService.GetJobsProcessingDetails(
                                                                (short)JobStatusType.Completed,
                                                                dateTimeUtc.AddMinutes(minutes),
                                                                dateTimeUtc,
                                                                cancellationToken);
            var model = new JobProcessingDetailViewModel
            {
                Data = jobDetails.ToList(),
                JobProcessingType = jobProcessingType
            };

            return model;
        }
    }
}