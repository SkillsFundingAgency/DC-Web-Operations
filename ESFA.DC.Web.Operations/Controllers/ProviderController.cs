using ESFA.DC.Logging.Interfaces;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.Web.Operations.Controllers
{
    public class ProviderController : BaseControllerWithAdvancedSupportPolicy
    {
        public ProviderController(ILogger logger, TelemetryClient telemetryClient)
            : base(logger, telemetryClient)
        {
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}