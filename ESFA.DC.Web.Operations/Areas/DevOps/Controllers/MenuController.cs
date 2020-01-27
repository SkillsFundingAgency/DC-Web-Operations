using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Controllers;
using ESFA.DC.Web.Operations.Utils;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.Web.Operations.Areas.DevOps.Controllers
{
    [Area(AreaNames.DevOps)]
    public class MenuController : BaseControllerWithDevOpsPolicy
    {
        public MenuController(ILogger logger, TelemetryClient telemetryClient)
            : base(logger, telemetryClient)
        {
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}