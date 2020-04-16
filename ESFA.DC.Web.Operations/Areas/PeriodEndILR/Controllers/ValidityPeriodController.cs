using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Areas.PeriodEndILR.Models;
using ESFA.DC.Web.Operations.Controllers;
using ESFA.DC.Web.Operations.Interfaces.PeriodEnd;
using ESFA.DC.Web.Operations.Utils;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.Web.Operations.Areas.PeriodEnd.Controllers
{
    [Area(AreaNames.PeriodEndILR)]
    [Route(AreaNames.PeriodEndILR + "/validityPeriod")]
    public class ValidityPeriodController : BaseControllerWithOpsPolicy
    {
        private readonly ILogger _logger;
        private readonly IPeriodService _periodService;

        public ValidityPeriodController(IPeriodService periodService, ILogger logger, TelemetryClient telemetryClient)
            : base(logger, telemetryClient)
        {
            _periodService = periodService;
        }

        public async Task<IActionResult> Index()
        {
            var model = new ValidityPeriodViewModel
            {
                Period = (await _periodService.ReturnPeriod(CollectionTypes.ILR))?.Period ?? 1
            };

            return View(model);
        }
    }
}