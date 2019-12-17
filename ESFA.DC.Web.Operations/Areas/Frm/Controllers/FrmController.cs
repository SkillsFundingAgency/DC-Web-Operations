using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ESFA.DC.Web.Operations.Areas.Frm.Models;
using ESFA.DC.Web.Operations.Areas.Provider.Models;
using ESFA.DC.Web.Operations.Utils;
using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.Web.Operations.Areas.Frm.Controllers
{
    [Area(AreaNames.Frm)]
    public class FrmController : Controller
    {
        public IActionResult Index()
        {
            var model = new FrmReportModel();
            model.IsFrmReportChoice = false;
            return View("Index", model);
        }

        public IActionResult SelectValidate()
        {
            return View("SelectValidate");
        }

        public async Task<IActionResult> SubmitFrm(FrmReportModel model)
        {
            if (ModelState.IsValid)
            {
                return View("HoldingPage");
            }

            return View("Index");
        }

        [HttpPost]
        public async Task<IActionResult> ReportChoiceSelection(FrmReportModel model)
        {
            if (model.IsFrmReportChoice)
            {
                return RedirectToAction("SelectValidate");
            }

            return View("Index");
        }
    }
}