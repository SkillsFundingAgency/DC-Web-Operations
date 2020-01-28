using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Constants.Authorization;
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

        [Route("claims/ops")]
        [Authorize(Policy = AuthorisationPolicy.OpsPolicy)]
        public IActionResult Ops()
        {
            Message = "Ops";
            _telemetryClient.TrackEvent($"Authed User : {User.Identity.Name} has {Message} Claim.");
            return View("Index");
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