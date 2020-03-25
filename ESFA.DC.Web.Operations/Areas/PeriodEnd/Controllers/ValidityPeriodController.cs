using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Controllers;
using ESFA.DC.Web.Operations.Interfaces.PeriodEnd;
using ESFA.DC.Web.Operations.Utils;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.Web.Operations.Areas.PeriodEnd.Controllers
{
    [Area(AreaNames.PeriodEnd)]
    [Route(AreaNames.PeriodEnd + "/validityPeriod")]
    public class ValidityPeriodController : BaseControllerWithOpsPolicy
    {
        private readonly ILogger _logger;
        private readonly IValidityPeriodService _validityPeriodService;

        public ValidityPeriodController(ILogger logger, TelemetryClient telemetryClient)
            : base(logger, telemetryClient)
        {
        }

        public async Task<IActionResult> Index()
        {
            return View("Index");
        }
    }
}