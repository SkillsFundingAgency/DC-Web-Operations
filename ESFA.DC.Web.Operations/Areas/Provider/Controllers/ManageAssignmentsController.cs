using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Areas.Provider.Models;
using ESFA.DC.Web.Operations.Interfaces.Provider;
using ESFA.DC.Web.Operations.Models.Collection;
using ESFA.DC.Web.Operations.Utils;
using Microsoft.AspNetCore.Mvc;
using MoreLinq;

namespace ESFA.DC.Web.Operations.Areas.Provider.Controllers
{
    [Area(AreaNames.Provider)]
    public class ManageAssignmentsController : Controller
    {
        private readonly ILogger _logger;
        private readonly IManageAssignmentsService _manageAssignmentsService;

        public ManageAssignmentsController(ILogger logger, IManageAssignmentsService manageAssignmentsService)
        {
            _logger = logger;
            _manageAssignmentsService = manageAssignmentsService;
        }

        public async Task<IActionResult> Index(long ukprn)
        {
            var model = new ManageAssignmentsViewModel();
            var provider = await _manageAssignmentsService.GetProvider(ukprn);
            var providerAssignments = await _manageAssignmentsService.GetProviderAssignments(ukprn);
            var availableCollections = await _manageAssignmentsService.GetAvailableCollections();

            model.Ukprn = ukprn;
            model.ProviderName = provider.Name;
            model.CollectionsAssignments = CreateOrderedList(providerAssignments, availableCollections);

            return View("Index", model);
        }

        [HttpPost]
        public async Task<IActionResult> Submit(ManageAssignmentsViewModel model)
        {
            for (var i = 0; i < model.CollectionsAssignments.Count; i++)
            {
                if (model.CollectionsAssignments[i].EndDate <= model.CollectionsAssignments[i].StartDate)
                {
                    ModelState.AddModelError($"Summary", $"{model.CollectionsAssignments[i].Name} - end date should be after start date");
                }
            }

            if (!ModelState.IsValid)
            {
                return View("Index", model);
            }

            if (await _manageAssignmentsService.UpdateProviderAssignments(model.Ukprn, model.CollectionsAssignments))
            {
                return RedirectToAction("Index", "ManageProviders", new { ukprn = model.Ukprn });
            }
            else
            {
                ModelState.AddModelError("Summary", "Error occured updating provider assignments");
                return View("Index", model);
            }
        }

        private IList<CollectionAssignment> CreateOrderedList(IEnumerable<CollectionAssignment> providerAssignments, IEnumerable<CollectionAssignment> availableCollections)
        {
            var i = 0;
            var sortedCollectionAssignments = new List<CollectionAssignment>();

            var sortedAssignments = providerAssignments.OrderByDescending(o => o.StartDate).ThenBy(t => t.DisplayOrder);

            foreach (var assignment in sortedAssignments)
            {
                assignment.DisplayOrder = i;
                sortedCollectionAssignments.Add(assignment);
                i++;
            }

            var sortedCollections = availableCollections
                .Where(a => !sortedAssignments.Any(s => s.CollectionId == a.CollectionId))
                .OrderBy(o => o.DisplayOrder);

            foreach (var collection in sortedCollections)
            {
                collection.DisplayOrder = i;
                sortedCollectionAssignments.Add(collection);
                i++;
            }

            return sortedCollectionAssignments;
        }
    }
}