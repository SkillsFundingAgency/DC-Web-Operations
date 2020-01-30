using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Constants.Authorization;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Authorization;

namespace ESFA.DC.Web.Operations.Controllers
{
    [Authorize(Policy = AuthorisationPolicy.OpsPolicy)]

    public abstract class BaseControllerWithOpsPolicy : BaseController
    {
        public BaseControllerWithOpsPolicy(ILogger logger, TelemetryClient telemetryClient)
            : base(logger, telemetryClient)
        {
        }
    }
}
