﻿using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Extensions;
using ESFA.DC.Web.Operations.Interfaces.ReferenceData;
using ESFA.DC.Web.Operations.Interfaces.Storage;
using ESFA.DC.Web.Operations.Utils;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.Web.Operations.Areas.ReferenceData.Controllers
{
    [Area(AreaNames.ReferenceData)]
    [Route(AreaNames.ReferenceData + "/conditionOfFundingRemoval")]
    public class ConditionOfFundingRemovalController : BaseReferenceDataController
    {
        private readonly IReferenceDataService _referenceDataService;

        public ConditionOfFundingRemovalController(
            IStorageService storageService,
            ILogger logger,
            TelemetryClient telemetryClient,
            IReferenceDataService referenceDataService)
            : base(storageService, logger, telemetryClient)
        {
            _referenceDataService = referenceDataService;
        }

        public async Task<IActionResult> Index()
        {
            var cancellationToken = CancellationToken.None;

            var model = await _referenceDataService.GetSubmissionsPerCollectionAsync(
                Utils.Constants.ReferenceDataStorageContainerName,
                CollectionNames.ReferenceDataConditionsOfFundingRemoval,
                ReportTypes.ConditionOfFundingRemovalReportName,
                cancellationToken: cancellationToken);

            return View(model);
        }

        [RequestSizeLimit(524_288_000)]
        [AutoValidateAntiforgeryToken]
        [HttpPost]
        public async Task<IActionResult> Index([FromForm] IFormFile file)
        {
            const int period = 0;

            if (file != null)
            {
                await _referenceDataService.SubmitJob(
                    period, CollectionNames.ReferenceDataConditionsOfFundingRemoval, User.Name(), User.Email(), file, CancellationToken.None);
            }

            return RedirectToAction("Index");
        }

        [Route("getReportFile/{fileName}/{jobId?}")]
        public async Task<FileResult> GetReportFileAsync(string fileName, long? jobId, CancellationToken cancellationToken)
        {
            return await GetReportFileAsync(CollectionNames.ReferenceDataConditionsOfFundingRemoval, fileName, jobId, cancellationToken);
        }
    }
}