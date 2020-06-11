using System;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Web.Operations.Areas.ReferenceData.Models;
using ESFA.DC.Web.Operations.Constants;
using ESFA.DC.Web.Operations.Interfaces;
using ESFA.DC.Web.Operations.Utils;
using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.Web.Operations.Areas.ReferenceData.Controllers
{
    [Area(AreaNames.ReferenceData)]
    [Route(AreaNames.ReferenceData + "/referenceData")]
    public class ReferenceDataController : Controller
    {
        private const string CreatedByPlaceHolder = "Data unavailable";

        private readonly IJobService _jobService;

        public ReferenceDataController(IJobService jobService)
        {
            _jobService = jobService;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var CITask = _jobService.GetLatestJobForCollectionAsync(CollectionNames.ReferenceDataCampusIdentifiers, cancellationToken);
            var CoFRTask = _jobService.GetLatestJobForCollectionAsync(CollectionNames.ReferenceDataConditionsOfFundingRemoval, cancellationToken);

            await Task.WhenAll(CITask, CoFRTask);

            var latestSuccessfulCIJob = CITask.Result;
            var latestSuccessfulCoFRJob = CoFRTask.Result;

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
                }
            };

            return View("Index", model);
        }
    }
}