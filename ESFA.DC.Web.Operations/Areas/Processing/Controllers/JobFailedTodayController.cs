using System.Threading.Tasks;
using ESFA.DC.Jobs.Model.Processing.JobsFailedToday;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Web.Operations.Controllers;
using ESFA.DC.Web.Operations.Interfaces.Processing;
using ESFA.DC.Web.Operations.Models.Processing;
using ESFA.DC.Web.Operations.Utils;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.Web.Operations.Areas.Processing.Controllers
{
    [Area(AreaNames.Processing)]
    [Route(AreaNames.Processing + "/jobFailedToday")]
    public class JobFailedTodayController : BaseController
    {
        private readonly IJobFailedTodayService _jobFailedTodayService;
        private readonly IJsonSerializationService _jsonSerializationService;

        public JobFailedTodayController(IJobFailedTodayService jobFailedTodayService, ILogger logger, TelemetryClient telemetryClient, IJsonSerializationService jsonSerializationService)
            : base(logger, telemetryClient)
        {
            _jobFailedTodayService = jobFailedTodayService;
            _jsonSerializationService = jsonSerializationService;
        }

        public async Task<IActionResult> Index()
        {
            var model = _jsonSerializationService.Deserialize<JobProcessingModel<JobFailedTodayLookupModel>>(await _jobFailedTodayService.GetJobsThatAreFailedToday());
            return View("Index", model);
        }
    }
}