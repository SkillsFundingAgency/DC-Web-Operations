using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Security.Policies;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Authorization;

namespace ESFA.DC.Web.Operations.Controllers
{
    [Authorize(Policy = AuthorisationPolicy.AdvancedSupportOrDevOpsPolicy)]

    public abstract class BaseControllerWithDevOpsOrAdvancedSupportPolicy : BaseController
    {
        public BaseControllerWithDevOpsOrAdvancedSupportPolicy(ILogger logger, TelemetryClient telemetryClient)
            : base(logger, telemetryClient)
        {
        }
    }
}
