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
            var provider = _manageAssignmentsService.GetProviderAsync(ukprn, cancellationToken);
            var availableCollections = _manageAssignmentsService.GetAvailableCollectionsAsync(cancellationToken);

            await Task.WhenAll(provider, availableCollections);

            var providerAssignments = await _manageAssignmentsService.GetActiveProviderAssignmentsAsync(ukprn, availableCollections.Result.ToList(), cancellationToken);

            var model = new ManageAssignmentsViewModel
            {
                Ukprn = ukprn,
                ProviderName = provider.Result.Name,
                ActiveCollectionsAssignments = providerAssignments.OrderByDescending(o => o.StartDate).ThenBy(t => t.DisplayOrder).ToList(),
                InactiveCollectionAssignments = availableCollections.Result.OrderBy(o => o.DisplayOrder).ExceptBy(providerAssignments, p => p.CollectionId).ToList()
            };

            return View("Index", model);
        }

        [HttpPost]
        public IActionResult Add(int collectionId, ManageAssignmentsViewModel model)
        {
            var record = model.InactiveCollectionAssignments.Single(s => s.CollectionId == collectionId);

            if (!record.StartDate.HasValue || !record.EndDate.HasValue || record.StartDate.Value.Year < 2000)
            {
                ModelState.Clear();
                record.StartDate = null;
                record.EndDate = null;
                ModelState.AddModelError($"Summary", $"Please supply a valid start and end date");
                return View("Index", model);
            }

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