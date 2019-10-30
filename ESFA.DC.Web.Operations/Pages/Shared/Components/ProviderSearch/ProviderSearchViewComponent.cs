using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ESFA.DC.Web.Operations.Areas.Provider.Models;
using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.Web.Operations.Components.ProviderSearch
{
    public class ProviderSearchViewComponent : ViewComponent
    {
        public ProviderSearchViewComponent()
        {
        }

        public IViewComponentResult Invoke()
        {
            var model = new ProviderViewModel();
            return View("Default", model);
        }
    }
}
