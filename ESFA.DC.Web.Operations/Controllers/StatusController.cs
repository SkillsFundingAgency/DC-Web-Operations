using ESFA.DC.Logging.Interfaces;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.Web.Operations.Controllers
{
    [AllowAnonymous]
    public class StatusController : BaseController
    {
        public StatusController(ILogger logger, TelemetryClient telemetryClient)
            : base(logger, telemetryClient)
        {
            Message = string.Empty;
        }

        [ViewData]
        public string Message { get; set; }

        public IActionResult Index()
        {
            Message = "OK";
            return View();
        }
    }
}