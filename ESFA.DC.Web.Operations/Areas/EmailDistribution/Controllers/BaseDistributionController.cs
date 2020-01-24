using ESFA.DC.Web.Operations.Constants;
using ESFA.DC.Web.Operations.Constants.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.Web.Operations.Areas.EmailDistribution.Controllers
{
    [Authorize(Policy = Constants.Authorization.AuthorisationPolicy.OpsPolicy)]
    public abstract class BaseDistributionController : Controller
    {
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
