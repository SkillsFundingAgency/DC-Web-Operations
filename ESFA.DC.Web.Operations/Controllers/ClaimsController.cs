using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Security.Policies;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.Web.Operations.Controllers
{
    public class ClaimsController : BaseController
    {
        private readonly ILogger _logger;
        private readonly TelemetryClient _telemetryClient;

        public ClaimsController(ILogger logger, TelemetryClient telemetryClient)
            : base(logger, telemetryClient)
        {
            _logger = logger;
            _telemetryClient = telemetryClient;
        }

        [ViewData]
        public string Message { get; set; }

        [Route("claims/index")]
        public IActionResult Index()
        {
            return View();
        }

        [Route("claims/devops")]
        [Authorize(Policy = AuthorisationPolicy.DevOpsPolicy)]
        public IActionResult DevOps()
        {
            Message = "Dev Ops";
            _telemetryClient.TrackEvent($"Authed User : {User.Identity.Name} has {Message} Claim.");
            return View("Index");
        }
    }
}