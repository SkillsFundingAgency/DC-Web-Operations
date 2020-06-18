﻿using System;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Areas.ReferenceData.Models;
using ESFA.DC.Web.Operations.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.Storage;
using ESFA.DC.Web.Operations.Utils;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.Web.Operations.Areas.ReferenceData.Controllers
{
    [Area(AreaNames.ReferenceData)]
    [Route(AreaNames.ReferenceData + "/referenceData")]
    public class ReferenceDataController : BaseReferenceDataController
    {
        private const string CreatedByPlaceHolder = "Data unavailable";

        private readonly IJobService _jobService;

        public ReferenceDataController(
            IStorageService storageService,
            ILogger logger,
            TelemetryClient telemetryClient,
            IJobService jobService)
            : base(storageService, logger, telemetryClient)
        {
            _jobService = jobService;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var CITask = _jobService.GetLatestJobForCollectionAsync(CollectionNames.ReferenceDataCampusIdentifiers, cancellationToken);
            var CoFRTask = _jobService.GetLatestJobForCollectionAsync(CollectionNames.ReferenceDataConditionsOfFundingRemoval, cancellationToken);
            var Val2021Task = _jobService.GetLatestJobForCollectionAsync(CollectionNames.ReferenceDataValidationMessages2021, cancellationToken);

            await Task.WhenAll(CITask, CoFRTask, Val2021Task);

            var latestSuccessfulCIJob = CITask.Result;
            var latestSuccessfulCoFRJob = CoFRTask.Result;
            var latestSuccessfulVal2021Job = Val2021Task.Result;

            var model = new ReferenceDataIndexModel
            {
                CampusIdentifiers = new ReferenceDataIndexBase
                {
                    LastUpdatedDateTime = latestSuccessfulCIJob?.DateTimeCreatedUtc ?? DateTime.MinValue,
                    LastUpdatedByWho = latestSuccessfulCIJob?.CreatedBy ?? CreatedByPlaceHolder
                },
                ConditionOfFundingRemoval = new ReferenceDataIndexBase
                {
                    LastUpdatedDateTime = latestSuccessfulCoFRJob?.DateTimeCreatedUtc ?? DateTime.MinValue,
                    LastUpdatedByWho = latestSuccessfulCoFRJob?.CreatedBy ?? CreatedByPlaceHolder
                },
                ValidationMessages2021 = new ReferenceDataIndexBase
                {
                    LastUpdatedDateTime = latestSuccessfulVal2021Job?.DateTimeCreatedUtc ?? DateTime.MinValue,
                    LastUpdatedByWho = latestSuccessfulVal2021Job?.CreatedBy ?? CreatedByPlaceHolder
                }
            };

            return View("Index", model);
        }
    }
}