using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Constants;
using ESFA.DC.Web.Operations.Controllers;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Authorization;

namespace ESFA.DC.Web.Operations.Areas.EmailDistribution.Controllers
{
    [Authorize(Policy = Constants.Authorization.AuthorisationPolicy.AdvancedSupportOrDevOpsPolicy)]
    public abstract class BaseDistributionController : BaseControllerWithAdvancedSupportPolicy
    {
        private readonly ILogger _logger;

        public BaseDistributionController(ILogger logger, TelemetryClient telemetryClient)
            : base(logger, telemetryClient)
        {
        }

        protected void AddError(string key)
        {
            ModelState.AddModelError(key, ErrorMessageLookup.GetErrorMessage(key));
        }

        protected void AddError(string key, string message)
        {
            ModelState.AddModelError(key, message);
        }
    }
}
