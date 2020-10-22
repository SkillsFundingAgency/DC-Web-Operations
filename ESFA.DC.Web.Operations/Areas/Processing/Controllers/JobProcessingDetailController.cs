using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.Jobs.Model.Enums;
using ESFA.DC.Jobs.Model.Processing.JobsProcessing;
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
    [Route(AreaNames.Processing + "/jobProcessingdetail")]
    public class JobProcessingDetailController : BaseController
    {
        private const string JobProcessingTypeCurrentPeriod = "CurrentPeriod";
        private const string JobProcessingTypeLastHour = "LastHour";
        private const string JobProcessingTypeLastFiveMins = "LastFiveMins";
        private readonly IJobProcessingDetailService _jobProcessingDetailService;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IJsonSerializationService _jsonSerializationService;

        public JobProcessingDetailController(
            IJobProcessingDetailService jobProcessingDetailService,
            ILogger logger,
            IDateTimeProvider dateTimeProvider,
            TelemetryClient telemetryClient,
            IJsonSerializationService jsonSerializationService)
            : base(logger, telemetryClient)
        {
            _jobProcessingDetailService = jobProcessingDetailService;
            _dateTimeProvider = dateTimeProvider;
            _jsonSerializationService = jsonSerializationService;
        }

        [HttpGet("CurrentPeriod/{collectionYear?}")]
        public async Task<IActionResult> CurrentPeriod(int? collectionYear, CancellationToken cancellationToken)
        {
            var model = _jsonSerializationService.Deserialize<JobProcessingModel<JobProcessingDetailLookupModel>>(await _jobProcessingDetailService.GetJobsProcessingDetailsForCurrentPeriod((short)JobStatusType.Completed, cancellationToken));
            model.CollectionYear = collectionYear;
            model.JobProcessingType = JobProcessingTypeCurrentPeriod;
            return View("Index", model);
        }

        [HttpGet("LastFiveMins/{collectionYear?}")]
        public async Task<IActionResult> LastFiveMins(int collectionYear, CancellationToken cancellationToken)
        {
            var model = _jsonSerializationService.Deserialize<JobProcessingModel<JobProcessingDetailLookupModel>>(await _jobProcessingDetailService.GetJobsProcessingDetailsForCurrentPeriodLast5Mins((short)JobStatusType.Completed, cancellationToken));
            model.CollectionYear = collectionYear;
            model.JobProcessingType = JobProcessingTypeLastFiveMins;
            return View("Index", model);
        }

        [HttpGet("LastHour/{collectionYear?}")]
        public async Task<IActionResult> LastHour(int? collectionYear, CancellationToken cancellationToken)
        {
            var model = _jsonSerializationService.Deserialize<JobProcessingModel<JobProcessingDetailLookupModel>>(await _jobProcessingDetailService.GetJobsProcessingDetailsForCurrentPeriodLastHour((short)JobStatusType.Completed, cancellationToken));
            model.CollectionYear = collectionYear;
            model.JobProcessingType = JobProcessingTypeLastHour;
            return View("Index", model);
        }
    }
}