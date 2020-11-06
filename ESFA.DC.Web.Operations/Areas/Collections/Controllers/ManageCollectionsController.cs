using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Areas.Collections.Models;
using ESFA.DC.Web.Operations.Controllers;
using ESFA.DC.Web.Operations.Extensions;
using ESFA.DC.Web.Operations.Interfaces.Collections;
using ESFA.DC.Web.Operations.Models.Auditing;
using ESFA.DC.Web.Operations.Models.Collection;
using ESFA.DC.Web.Operations.Utils;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.Web.Operations.Areas.Collections.Controllers
{
    [Area(AreaNames.Collections)]
    public class ManageCollectionsController : BaseControllerWithDevOpsOrAdvancedSupportPolicy
    {
        private readonly ICollectionsService _collectionsService;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly ILogger _logger;

        public ManageCollectionsController(ICollectionsService collectionsService, IDateTimeProvider dateTimeProvider, ILogger logger, TelemetryClient telemetryClient)
            : base(logger, telemetryClient)
        {
            _logger = logger;
            _collectionsService = collectionsService;
            _dateTimeProvider = dateTimeProvider;
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
                CollectionId = collection.CollectionId,
                CollectionName = collection.CollectionTitle,
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
            var returnPeriod = await _collectionsService.GetReturnPeriod(id);

            var model = new ManageReturnPeriodViewModel()
            {
                ReturnPeriodId = id,
                CollectionName = returnPeriod.CollectionName,
                CollectionId = returnPeriod.CollectionId,
                PeriodName = returnPeriod.Name,
                OpeningDate = returnPeriod.OpenDate,
                OpeningTime = returnPeriod.OpenDate.ToString("HH:mm"),
                ClosingDate = returnPeriod.CloseDate,
                ClosingTime = returnPeriod.CloseDate.ToString("HH:mm")
            };

            return View("IndividualPeriod", model);
        }

        [HttpGet]
        public async Task<IActionResult> CollectionOverride(string collectionName)
        {
            var model = await PopulateCollectionOverrideViewModel(collectionName);

            return View("CollectionOverride", model);
        }

        [HttpGet]
        public async Task<IActionResult> FailJob(int jobId, string collectionName, CancellationToken cancellationToken)
        {
            var username = User.Name();
            var differentiator = DifferentiatorPath.FailJobDTO;
            var result = await _collectionsService.FailJob(jobId, cancellationToken, username, differentiator);

            var model = await PopulateCollectionOverrideViewModel(collectionName);

            if (!result)
            {
                ModelState.AddModelError("Summary", "Error 'Fail'ing Job");
            }

            return View("CollectionOverride", model);
        }

        [HttpPost]
        public async Task<IActionResult> SaveCollectionOverride(ManageCollectionsCollectionOverrideViewModel model)
        {
            bool? processingOverride = null;

            if (model != null)
            {
                switch (model.ProcessingOverride)
                {
                    case 1:
                        processingOverride = null;
                        break;

                    case 2:
                        processingOverride = true;
                        break;

                    case 3:
                        processingOverride = false;
                        break;
                }

                await _collectionsService.SetCollectionProcessingOverride(model.CollectionId, processingOverride);
            }

            return RedirectToAction("Index", new { collectionId = model.CollectionId });
        }

        [HttpPost]
        public async Task<IActionResult> IndividualCollectionsSubmit(ManageReturnPeriodViewModel model)
        {
            var openDateTime = model.OpeningDate.Add(TimeSpan.Parse(model.OpeningTime));
            var closeDateTime = model.ClosingDate.Add(TimeSpan.Parse(model.ClosingTime));

            if (openDateTime > closeDateTime)
            {
                ModelState.AddModelError("Summary", "Error - close date time should be greater than the open date time.");
                return View("IndividualPeriod", model);
            }

            var returnPeriod = new ReturnPeriod()
            {
                ReturnPeriodId = model.ReturnPeriodId,
                OpenDate = _dateTimeProvider.ConvertUkToUtc(openDateTime),
                CloseDate = _dateTimeProvider.ConvertUkToUtc(closeDateTime)
            };

            if (ModelState.IsValid && await _collectionsService.UpdateReturnPeriod(returnPeriod))
            {
                return RedirectToAction("Index", new { collectionId = model.CollectionId });
            }

            ModelState.AddModelError("Summary", "Error updating return period.");
            return View("IndividualPeriod", model);
        }

        private string SetOverrideValue(bool? processingOverride)
        {
            switch (processingOverride)
            {
                case true:
                    return "Force file processing";

                case false:
                    return "Stop processing";

                default:
                    return "Automatic";
            }
        }

        private async Task<ManageCollectionsCollectionOverrideViewModel> PopulateCollectionOverrideViewModel(string collectionName)
        {
            var collection = await _collectionsService.GetCollectionFromName(collectionName);

            var jobs = await _collectionsService.GetCollectionJobs(collectionName);

            var model = new ManageCollectionsCollectionOverrideViewModel()
            {
                CollectionName = collection.CollectionTitle,
                CollectionId = collection.CollectionId,
                Jobs = jobs.ToList()
            };

            switch (collection.ProcessingOverride)
            {
                case true:
                    // force file processing
                    model.ProcessingOverride = 2;
                    break;

                case false:
                    // stop processing
                    model.ProcessingOverride = 3;
                    break;

                default:
                    // automatic processing
                    model.ProcessingOverride = 1;
                    break;
            }

            return model;
        }
    }
}