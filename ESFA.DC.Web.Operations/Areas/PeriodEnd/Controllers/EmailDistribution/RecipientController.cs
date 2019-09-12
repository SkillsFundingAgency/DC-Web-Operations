using System.Threading.Tasks;
using ESFA.DC.Web.Operations.Interfaces.PeriodEnd;
using ESFA.DC.Web.Operations.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.Web.Operations.Areas.PeriodEnd.Controllers.EmailDistribution
{
    [Area(AreaNames.PeriodEnd)]
    [Route(AreaNames.PeriodEnd + "/distribution/recipient")]
    [Authorize]
    public class RecipientController : Controller
    {
        private readonly IEmailDistributionService _emailDistributionService;

        public RecipientController(IEmailDistributionService emailDistributionService)
        {
            _emailDistributionService = emailDistributionService;
        }

        public async Task<IActionResult> Index()
        {
            var data = await _emailDistributionService.GetEmailRecipientGroups();
            return View(data);
        }

        [HttpPost]
        public IActionResult Submit()
        {
            return View("Recipient");
            //return RedirectToAction("Index", new { collectionYear, period });
        }
    }
}