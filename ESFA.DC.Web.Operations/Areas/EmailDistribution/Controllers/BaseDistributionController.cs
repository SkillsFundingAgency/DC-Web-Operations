using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ESFA.DC.Web.Operations.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.Web.Operations.Areas.EmailDistribution.Controllers
{
    [Authorize]

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
