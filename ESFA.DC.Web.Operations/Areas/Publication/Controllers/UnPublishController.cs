using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Jobs.Model;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Areas.Publication.Models;
using ESFA.DC.Web.Operations.Constants;
using ESFA.DC.Web.Operations.Controllers;
using ESFA.DC.Web.Operations.Interfaces.Collections;
using ESFA.DC.Web.Operations.Interfaces.Publication;
using ESFA.DC.Web.Operations.Interfaces.Storage;
using ESFA.DC.Web.Operations.Utils;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.Web.Operations.Areas.Publication.Controllers
{
    [Area(AreaNames.Publication)]
    public class UnPublishController : BaseControllerWithDevOpsOrAdvancedSupportPolicy
    {
        private readonly IReportsPublicationService _reportsPublicationService;
        private readonly ICollectionsService _collectionsService;
        private readonly ILogger _logger;

        public UnPublishController(
            ILogger logger,
            IReportsPublicationService reportsPublicationService,
            IStorageService storageService,
            ICollectionsService collectionsService,
            TelemetryClient telemetryClient)
            : base(logger, telemetryClient)
        {
            _logger = logger;
            _reportsPublicationService = reportsPublicationService;
            _collectionsService = collectionsService;
        }

        public async Task<IActionResult> Index()
        {
            var collections = (await _collectionsService.GetCollectionsByType(CollectionTypeConstants.Publication)).ToList();
            ViewData[ViewDataConstants.Collections] = collections.Select(x => x.CollectionTitle);

            return await ReportTypeChanged(new PublicationReportModel()
            {
                CollectionName = collections.FirstOrDefault()?.CollectionTitle,
            });
        }

        public async Task<IActionResult> ReportTypeChanged(PublicationReportModel model)
        {
            var collectionType = CollectionTypeConstants.Publication;
            var reportsData = await _reportsPublicationService.GetReportsDataAsync(model.CollectionName);
            var lastTwoYears = await _reportsPublicationService.GetLastTwoCollectionYearsAsync(collectionType);
            var lastYearValue = lastTwoYears.LastOrDefault();
            model.PublishedFrm = reportsData.Where(x => x.CollectionYear == lastYearValue); // get all the open periods from the latest year period

            if (lastTwoYears.Count() > 1) //if there are more than two years in the collection
            {
                var firstYearValue = lastTwoYears.First();
                var firstYearList = reportsData.Where(x => x.CollectionYear == firstYearValue).TakeLast(1); //take the most recent open period in the previous year
                model.PublishedFrm = firstYearList.Concat(model.PublishedFrm); // add it to the front of the list
            }

            var collections = await _collectionsService.GetCollectionsByType(CollectionTypeConstants.Publication);
            ViewData[ViewDataConstants.Collections] = collections.Select(x => x.CollectionTitle);

            return View("Index", model);
        }

        public async Task<IActionResult> UnpublishAsync(CancellationToken cancellationToken, int periodNumber, string collectionName)
        {
            try
            {
                var collection = await _collectionsService.GetCollectionAsync(collectionName, cancellationToken);

                await _reportsPublicationService.UnpublishSldAsync(collectionName, periodNumber);

                await _reportsPublicationService.UnpublishSldDeleteFolderAsync(collection.StorageReference, periodNumber);
                return View("UnpublishSuccess");
            }
            catch (Exception ex)
            {
                string errorMessage = $"The FRM Reports were not able to be unpublished from SLD";
                _logger.LogError(errorMessage, ex);
                TempData["Error"] = errorMessage;
                return View("ErrorView");
            }
        }
    }
}
