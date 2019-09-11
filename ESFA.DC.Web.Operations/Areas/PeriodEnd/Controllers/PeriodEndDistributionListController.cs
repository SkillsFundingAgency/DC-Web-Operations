using ESFA.DC.Web.Operations.Utils;
using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.Web.Operations.Areas.PeriodEnd.Controllers
{
    [Area(AreaNames.PeriodEnd)]
    [Route(AreaNames.PeriodEnd + "/PeriodEndDistributionList")]
    public class PeriodEndDistributionListController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}