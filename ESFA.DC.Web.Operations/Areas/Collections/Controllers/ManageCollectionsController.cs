using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Areas.Collections.Models;
using ESFA.DC.Web.Operations.Interfaces.Collections;
using ESFA.DC.Web.Operations.Models.Collection;
using ESFA.DC.Web.Operations.Utils;
using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.Web.Operations.Areas.Collections.Controllers
{
    [Area(AreaNames.Collections)]
    public class ManageCollectionsController : Controller
    {
        private readonly ICollectionsService _collectionsService;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly ILogger _logger;

        public ManageCollectionsController(ICollectionsService collectionsService, IDateTimeProvider dateTimeProvider, ILogger logger)
        {
            _collectionsService = collectionsService;
            _dateTimeProvider = dateTimeProvider;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = new ManageCollectionsViewModel();
            var collections = new List<CollectionSummary>();

            var collectionYears = await _collectionsService.GetAvailableCollectionYears();
            foreach (var collectionYear in collectionYears)
            {
                var collectionSummaries = await _collectionsService.GetAllCollectionSummariesForYear(collectionYear);
                collections.AddRange(collectionSummaries);
            }

            model.Collections = collections;

            return View("Index", model);
        }

        [HttpGet("{collectionId}")]
        public async Task<IActionResult> Index(int collectionId)
        {
            var collection = await _collectionsService.GetCollectionById(collectionId);
            var returnPeriods = await _collectionsService.GetReturnPeriodsForCollection(collectionId);

            var now = _dateTimeProvider.GetNowUtc();

            var currentPeriod = returnPeriods.SingleOrDefault(rp => rp.OpenDate <= now && rp.CloseDate >= now);
            if (currentPeriod == null && now <= returnPeriods.Max(rp => rp.CloseDate))
            {
                // in between open periods - set current period to next available one.
                currentPeriod = returnPeriods.OrderBy(o => o.OpenDate).FirstOrDefault(w => w.OpenDate > now);
            }

            var model = new ManageCollectionViewModel()
            {
                CollectionId = collection.Id,
                CollectionName = collection.Name,
                ClosingDate = currentPeriod?.CloseDate,
                CurrentPeriod = currentPeriod?.Name,
                ProcessingOverride = SetOverrideValue(collection.ProcessingOverride),
                ReturnPeriods = returnPeriods.ToList()
            };

            if (currentPeriod != null)
            {
                model.DaysRemaining = (currentPeriod.CloseDate - now).Days;
            }

            return View("IndividualCollection", model);
        }

        [HttpGet("returnperiod/{id}")]
        public async Task<IActionResult> ManageReturnPeriod(int id)
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        public async Task<IActionResult> CollectionOverride()
        {
            throw new NotImplementedException();
        }

        private string SetOverrideValue(bool? processingOverride)
        {
            switch (processingOverride)
            {
                case true:
                    return "Stop processing";
                case false:
                    return "Force file processing";
                default:
                    return "Automatic";
            }
        }
    }
}