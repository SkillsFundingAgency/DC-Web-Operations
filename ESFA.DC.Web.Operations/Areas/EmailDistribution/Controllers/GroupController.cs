using System.Threading.Tasks;
using ESFA.DC.Web.Operations.Interfaces.PeriodEnd;
using ESFA.DC.Web.Operations.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.Web.Operations.Areas.EmailDistribution.Controllers
{
    [Area(AreaNames.EmailDistribution)]
    [Route(AreaNames.EmailDistribution + "/group")]
    [Authorize]
    public class GroupController : Controller
    {
        private readonly IEmailDistributionService _emailDistributionService;

        public GroupController(IEmailDistributionService emailDistributionService)
        {
            _emailDistributionService = emailDistributionService;
        }

        public async Task<IActionResult> Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Submit(string groupName)
        {
            await _emailDistributionService.SaveGroup(groupName);
            return RedirectToAction("Index", "List");
        }
    }
}