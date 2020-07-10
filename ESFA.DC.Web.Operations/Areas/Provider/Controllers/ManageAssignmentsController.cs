using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Areas.Provider.Models;
using ESFA.DC.Web.Operations.Controllers;
using ESFA.DC.Web.Operations.Interfaces.Provider;
using ESFA.DC.Web.Operations.Models.Collection;
using ESFA.DC.Web.Operations.Utils;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;
using MoreLinq;

namespace ESFA.DC.Web.Operations.Areas.Provider.Controllers
{
    [Area(AreaNames.Provider)]
    public class ManageAssignmentsController : BaseControllerWithDevOpsOrAdvancedSupportPolicy
    {
        private readonly ILogger _logger;
        private readonly IManageAssignmentsService _manageAssignmentsService;

        public ManageAssignmentsController(ILogger logger, IManageAssignmentsService manageAssignmentsService, TelemetryClient telemetryClient)
            : base(logger, telemetryClient)
        {
            _logger = logger;
            _manageAssignmentsService = manageAssignmentsService;
        }

        public async Task<IActionResult> Index(long ukprn, CancellationToken cancellationToken)
        {
            var model = new ManageAssignmentsViewModel();

            var provider = await _manageAssignmentsService.GetProviderAsync(ukprn, cancellationToken);
            var providerAssignments = await _manageAssignmentsService.GetProviderAssignmentsAsync(ukprn, cancellationToken);
            var availableCollections = (await _manageAssignmentsService.GetAvailableCollectionsAsync(cancellationToken)).ExceptBy(providerAssignments, p => p.CollectionId);

            model.Ukprn = ukprn;
            model.ProviderName = provider.Name;
            model.ActiveCollectionsAssignments = providerAssignments.OrderByDescending(o => o.StartDate).ThenBy(t => t.DisplayOrder).ToList();
            model.InactiveCollectionAssignments = availableCollections.OrderBy(o => o.DisplayOrder).ToList();

            return View("Index", model);
        }

        [HttpPost]
        public IActionResult Remove(int collectionId, ManageAssignmentsViewModel model)
        {
            var record = model.ActiveCollectionsAssignments.Single(s => s.CollectionId == collectionId);
            record.StartDate = null;
            record.EndDate = null;
            model.ActiveCollectionsAssignments.Remove(record);
            model.InactiveCollectionAssignments.Add(record);
            model.InactiveCollectionAssignments = model.InactiveCollectionAssignments.OrderBy(o => o.DisplayOrder).ToList();
            ModelState.Clear();

            return View("Index", model);
        }

        [HttpPost]
        public IActionResult Add(int collectionId, ManageAssignmentsViewModel model)
        {
            var record = model.InactiveCollectionAssignments.Single(s => s.CollectionId == collectionId);
            model.ActiveCollectionsAssignments.Add(record);
            model.InactiveCollectionAssignments.Remove(record);
            model.ActiveCollectionsAssignments = model.ActiveCollectionsAssignments.OrderByDescending(o => o.StartDate).ThenBy(t => t.DisplayOrder).ToList();
            ModelState.Clear();

            return View("Index", model);
        }

        [HttpPost]
        public async Task<IActionResult> Submit(ManageAssignmentsViewModel model, CancellationToken cancellationToken)
        {
            foreach (var assignment in model.ActiveCollectionsAssignments)
            {
                if (!assignment.StartDate.HasValue && !assignment.EndDate.HasValue)
                {
                    assignment.ToBeDeleted = true;
                }
                else if (assignment.EndDate <= assignment.StartDate)
                {
                    ModelState.AddModelError($"Summary", $"{assignment.Name} - end date should be after start date");
                }
            }

            if (!ModelState.IsValid)
            {
                return View("Index", model);
            }

            model.InactiveCollectionAssignments.ForEach(f => f.ToBeDeleted = true);

            var allAssignments = new List<CollectionAssignment>();
            allAssignments.AddRange(model.ActiveCollectionsAssignments);
            allAssignments.AddRange(model.InactiveCollectionAssignments);

            if (await _manageAssignmentsService.UpdateProviderAssignmentsAsync(model.Ukprn, allAssignments, cancellationToken))
            {
                return RedirectToAction("Index", "ManageProviders", new { ukprn = model.Ukprn });
            }

            ModelState.AddModelError("Summary", "Error occured updating provider assignments");
            return View("Index", model);
        }
    }
}