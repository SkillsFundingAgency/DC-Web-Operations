﻿using ESFA.DC.Web.Operations.Interfaces.PeriodEnd;
using ESFA.DC.Web.Operations.Utils;
using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.Web.Operations.Areas.PeriodEnd.Controllers
{
    [Area(AreaNames.PeriodEnd)]
    [Route(AreaNames.PeriodEnd + "/PeriodEndDistributionList")]
    public class PeriodEndDistributionListController : Controller
    {
        private readonly IEmailDistributionService _emailDistributionService;

        public PeriodEndDistributionListController(
            IEmailDistributionService emailDistributionService)
        {
            _emailDistributionService = emailDistributionService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Route("editMailingList/{id}")]
        public IActionResult EditMailingList(int id)
        {
            return RedirectToAction("Index");
        }

        [Route("removeMailingList/{id}")]
        public IActionResult DeleteMailingList(int id)
        {
            return RedirectToAction("Index");
        }
    }
}