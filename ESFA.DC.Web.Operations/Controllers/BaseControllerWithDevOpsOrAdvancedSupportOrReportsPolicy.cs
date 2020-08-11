using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Security.Policies;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Authorization;

namespace ESFA.DC.Web.Operations.Controllers
{
    [Authorize(Policy = AuthorisationPolicy.AdvancedSupportOrDevOpsOrReportsPolicy)]

    public abstract class BaseControllerWithDevOpsOrAdvancedSupportOrReportsPolicy : BaseController
    {
        public BaseControllerWithDevOpsOrAdvancedSupportOrReportsPolicy(ILogger logger, TelemetryClient telemetryClient)
            : base(logger, telemetryClient)
        {
        }
    }
}
