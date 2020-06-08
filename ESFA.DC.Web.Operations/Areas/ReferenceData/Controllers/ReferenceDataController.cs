using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Jobs.Model;
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
        private readonly IJobService _jobService;

        public ReferenceDataController(IJobService jobService)
        {
            _jobService = jobService;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var latestSuccessfulJob = await _jobService.GetLatestJobForCollectionAsync(CollectionNames.ReferenceDataCampusIdentifiers, cancellationToken) ??
                                      new SubmittedJob()
                                          { DateTimeCreatedUtc = DateTime.MinValue, CreatedBy = "Data unavailable" };

            var model = new ReferenceDataIndexModel()
            {
                CampusIdentifiers = new ReferenceDataIndexBase()
                {
                    LastUpdatedDateTime = latestSuccessfulJob.DateTimeCreatedUtc,
                    LastUpdatedByWho = latestSuccessfulJob.CreatedBy
                }
            };

            return View("Index", model);
        }
    }
}