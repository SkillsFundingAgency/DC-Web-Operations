using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ESFA.DC.Web.Operations.Interfaces.PeriodEnd;
using ESFA.DC.Web.Operations.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.Web.Operations.Areas.EmailDistribution.Controllers
{
    [Area(AreaNames.EmailDistribution)]
    [Route(AreaNames.EmailDistribution)]
    [Authorize]
    public class ListController : Controller
    {
        private readonly IEmailDistributionService _emailDistributionService;

        public ListController(IEmailDistributionService emailDistributionService)
        {
            _emailDistributionService = emailDistributionService;
        }

        public async Task<IActionResult> Index()
        {
            var data = await _emailDistributionService.GetEmailRecipientGroups();
            return View(data);
        }
    }
}