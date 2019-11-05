using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Areas.Provider.Models;
using ESFA.DC.Web.Operations.Interfaces.Provider;
using ESFA.DC.Web.Operations.Utils;
using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.Web.Operations.Areas.Provider.Controllers
{
    [Area(AreaNames.Provider)]
    public class AddNewController : Controller
    {
        private readonly ILogger _logger;
        private readonly IAddNewProviderService _addNewProviderService;

        public AddNewController(ILogger logger, IAddNewProviderService addNewProviderService)
        {
            _logger = logger;
            _addNewProviderService = addNewProviderService;
        }

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
            _logger.LogDebug("Entered AddSingleProvider");

            const string DuplicateOrganisation = "Duplicate Organisation exists.";

            if (!ModelState.IsValid)
            {
                return View("Index", model);
            }

            var response = await _addNewProviderService.SaveProvider(model.ProviderName, model.Ukprn, model.Upin, model.IsMca, CancellationToken.None);

            if (response.StatusCode == 409)
            {
                ModelState.AddModelError("Summary", DuplicateOrganisation);
                return View("Index", model);
            }

            _logger.LogDebug("Exit AddSingleProvider");
            return RedirectToAction("Index", "ManageAssignments");
        }
    }
}