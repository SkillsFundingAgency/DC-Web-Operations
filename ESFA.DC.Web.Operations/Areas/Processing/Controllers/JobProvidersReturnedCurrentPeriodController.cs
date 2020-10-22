using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Jobs.Model.Processing.ProvidersReturned;
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
    [Route(AreaNames.Processing + "/jobProvidersReturnedCurrentPeriod")]
    public class JobProvidersReturnedCurrentPeriodController : BaseController
    {
        private readonly IJobProvidersReturnedCurrentPeriodService _JobProvidersReturnedCurrentPeriodService;
        private readonly IJsonSerializationService _jsonSerializationService;

        public JobProvidersReturnedCurrentPeriodController(
            IJobProvidersReturnedCurrentPeriodService jobProvidersReturnedCurrentPeriodService,
            ILogger logger,
            TelemetryClient telemetryClient,
            IJsonSerializationService jsonSerializationService)
            : base(logger, telemetryClient)
        {
            _JobProvidersReturnedCurrentPeriodService = jobProvidersReturnedCurrentPeriodService;
            _jsonSerializationService = jsonSerializationService;
        }

        public async Task<IActionResult> Index(int? collectionYear, CancellationToken cancellationToken)
        {
            var model = _jsonSerializationService.Deserialize<JobProcessingModel<ProvidersReturnedCurrentPeriodLookupModel>>(await _JobProvidersReturnedCurrentPeriodService.GetProvidersReturnedCurrentPeriodAsync(cancellationToken));
            model.CollectionYear = collectionYear;

            return View("Index", model);
        }
    }
}