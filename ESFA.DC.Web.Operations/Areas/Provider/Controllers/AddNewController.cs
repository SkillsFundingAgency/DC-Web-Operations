using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ESFA.DC.Web.Operations.Areas.Provider.Models;
using ESFA.DC.Web.Operations.Utils;
using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.Web.Operations.Areas.Provider.Controllers
{
    [Area(AreaNames.Provider)]
    public class AddNewController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> AddNew()
        {
            var model = new ProviderViewModel();
            model.IsSingleAddNewProviderChoice = true;
            return View(model);
        }

        public async Task<IActionResult> LoadBulk()
        {
            return View("Index");
        }

        [HttpPost]
        public async Task<IActionResult> AddNewChoiceSubmit(ProviderViewModel model)
        {
           if (model.IsSingleAddNewProviderChoice)
           {
               return RedirectToAction("Index");
           }

           return RedirectToAction("LoadBulk");
        }

        [HttpPost]
        public async Task<IActionResult> AddSingleProvider(ProviderViewModel model)
        {
            return View("Index");
        }
    }
}