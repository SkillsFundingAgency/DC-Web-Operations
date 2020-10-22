using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Jobs.Model.Processing.JobsFailedCurrentPeriod;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Web.Operations.Areas.Processing.Models;
using ESFA.DC.Web.Operations.Controllers;
using ESFA.DC.Web.Operations.Interfaces.Processing;
using ESFA.DC.Web.Operations.Models.Processing;
using ESFA.DC.Web.Operations.Utils;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.Web.Operations.Areas.Processing.Controllers
{
    [Area(AreaNames.Processing)]
    [Route(AreaNames.Processing + "/jobFailedCurrentPeriod")]
    public class JobFailedCurrentPeriodController : BaseController
    {
        private readonly IJobFailedCurrentPeriodService _jobFailedCurrentPeriodService;
        private readonly IJsonSerializationService _jsonSerializationService;

        public JobFailedCurrentPeriodController(
            IJobFailedCurrentPeriodService jobFailedCurrentPeriodService,
            ILogger logger,
            TelemetryClient telemetryClient,
            IJsonSerializationService jsonSerializationService)
            : base(logger, telemetryClient)
        {
            _jobFailedCurrentPeriodService = jobFailedCurrentPeriodService;
            _jsonSerializationService = jsonSerializationService;
        }

        public async Task<IActionResult> Index(int? collectionYear, CancellationToken cancellationToken)
        {
            var model = _jsonSerializationService.Deserialize<JobProcessingModel<JobsFailedCurrentPeriodLookupModel>>(await _jobFailedCurrentPeriodService.GetJobsFailedCurrentPeriodAsync(cancellationToken));
            model.CollectionYear = collectionYear;

            return View("Index", model);
        }
    }
}