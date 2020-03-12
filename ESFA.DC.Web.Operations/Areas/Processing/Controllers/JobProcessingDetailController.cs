using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            var model = new JobProcessingDetailViewModel();
            model.Data = await _jobProcessingDetailService.GetJobsProcessingDetails(DateTime.UtcNow.AddDays(-100), DateTime.UtcNow);
            model.JobProcessingType = "CurrentPeriod";
            return View("Index", model);
        }

        [HttpGet("LastHour")]
        public async Task<IActionResult> LastHour()
        {
            var model = new JobProcessingDetailViewModel();
            model.Data = await _jobProcessingDetailService.GetJobsProcessingDetails(DateTime.UtcNow.AddHours(-1), DateTime.UtcNow);
            model.JobProcessingType = "LastHour";
            return View("Index", model);
        }

        [HttpGet("LastFiveMins")]
        public async Task<IActionResult> LastFiveMins()
        {
            var model = new JobProcessingDetailViewModel();
            model.Data = await _jobProcessingDetailService.GetJobsProcessingDetails(DateTime.UtcNow.AddMinutes(-5), DateTime.UtcNow);
            model.JobProcessingType = "LastFiveMins";
            return View("Index", model);
        }
    }
}