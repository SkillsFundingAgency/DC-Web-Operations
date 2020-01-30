using ESFA.DC.Logging.Interfaces;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.Web.Operations.Controllers
{
    public abstract class BaseController : Controller
    {
        private readonly ILogger _logger;
        private readonly TelemetryClient _telemetryClient;

        public BaseController(ILogger logger, TelemetryClient telemetryClient)
        {
            _logger = logger;
            _telemetryClient = telemetryClient;
        }
    }
}
