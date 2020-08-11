using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Security.Policies;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Authorization;

namespace ESFA.DC.Web.Operations.Controllers
{
    [Authorize(Policy = AuthorisationPolicy.ReportsPolicy)]

    public abstract class BaseControllerWithReportsPolicy : BaseController
    {
        public BaseControllerWithReportsPolicy(ILogger logger, TelemetryClient telemetryClient)
            : base(logger, telemetryClient)
        {
        }
    }
}
