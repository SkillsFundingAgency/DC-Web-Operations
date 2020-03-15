using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ESFA.DC.Jobs.Model.Enums;
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

        public JobProcessingDetailController(IJobProcessingDetailService jobProcessingDetailService, ILogger logger, TelemetryClient telemetryClient)
            : base(logger, telemetryClient)
        {
            _jobProcessingDetailService = jobProcessingDetailService;
            _logger = logger;
        }

        [HttpGet("CurrentPeriod")]
        public async Task<IActionResult> CurrentPeriod()
        {
            var model = new JobProcessingDetailViewModel
            {
                Data = await _jobProcessingDetailService.GetJobsProcessingDetailsForCurrentPeriod((short)JobStatusType.Completed),
                JobProcessingType = JobProcessingTypeCurrentPeriod
            };
            return View("Index", model);
        }

        [HttpGet("LastHour")]
        public async Task<IActionResult> LastHour()
        {
            var model = new JobProcessingDetailViewModel
            {
                Data = await _jobProcessingDetailService.GetJobsProcessingDetails((short)JobStatusType.Completed, DateTime.UtcNow.AddHours(-1), DateTime.UtcNow),
                JobProcessingType = JobProcessingTypeLastHour
            };
            return View("Index", model);
        }

        [HttpGet("LastFiveMins")]
        public async Task<IActionResult> LastFiveMins()
        {
            var model = new JobProcessingDetailViewModel
            {
                Data = await _jobProcessingDetailService.GetJobsProcessingDetails((short)JobStatusType.Completed, DateTime.UtcNow.AddMinutes(-5), DateTime.UtcNow),
                JobProcessingType = JobProcessingTypeLastFiveMins
            };
            return View("Index", model);
        }
    }
}