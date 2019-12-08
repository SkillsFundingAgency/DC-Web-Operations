using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.Web.Operations.ViewComponents
{
    public class ProviderSearchViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(string searchType)
        {
            return View("Default", searchType);
        }
    }
}
