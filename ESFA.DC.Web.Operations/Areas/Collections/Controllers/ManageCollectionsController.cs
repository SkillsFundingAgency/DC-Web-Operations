using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        private readonly ILogger _logger;

        public ManageCollectionsController(ICollectionsService collectionsService, ILogger logger)
        {
            _collectionsService = collectionsService;
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
            throw new NotImplementedException();
        }
    }
}