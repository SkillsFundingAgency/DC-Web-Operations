using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Areas.PeriodEndALLF.Models;
using ESFA.DC.Web.Operations.Constants;
using ESFA.DC.Web.Operations.Controllers;
using ESFA.DC.Web.Operations.Extensions;
using ESFA.DC.Web.Operations.Interfaces.PeriodEnd;
using ESFA.DC.Web.Operations.Models;
using ESFA.DC.Web.Operations.Utils;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.Web.Operations.Areas.PeriodEndALLF.Controllers
{
    [Area(AreaNames.PeriodEndALLF)]
    [Route(AreaNames.PeriodEndALLF + "/periodEnd")]
    public class PeriodEndController : BaseControllerWithOpsPolicy
    {
        private readonly IALLFPeriodEndService _periodEndService;
        private readonly IPeriodService _periodService;

        public PeriodEndController(
            IALLFPeriodEndService periodEndService,
            IPeriodService periodService,
            ILogger logger,
            TelemetryClient telemetryClient)
            : base(logger, telemetryClient)
        {
            _periodEndService = periodEndService;
            _periodService = periodService;
        }

        [HttpGet("{collectionYear?}/{period?}")]
        public async Task<IActionResult> Index(int? collectionYear, int? period, CancellationToken cancellationToken)
        {
            var currentYearPeriod = await _periodService.ReturnPeriod(CollectionTypes.ALLF, cancellationToken);
            currentYearPeriod.Year = currentYearPeriod.Year ?? 0;

            var model = new PeriodEndViewModel();

            if (collectionYear != null && period != null)
            {
                model.Files = (await GetSubmittedFiles(collectionYear.Value, period.Value, cancellationToken)).ToList();
                model.Year = collectionYear.Value;
                model.Period = period.Value;
            }
            else
            {
                model.Files = (await GetSubmittedFiles(currentYearPeriod.Year.Value, currentYearPeriod.Period, cancellationToken)).ToList();
                model.Year = currentYearPeriod.Year.Value;
                model.Period = currentYearPeriod.Period;
            }

            var isCurrentPeriodSelected = currentYearPeriod.Year == model.Year && currentYearPeriod.Period == model.Period;
            model.IsCurrentPeriod = isCurrentPeriodSelected;
            model.CollectionClosed = isCurrentPeriodSelected && currentYearPeriod.PeriodClosed;

            return View(model);
        }

        [HttpPost]
        [RequestSizeLimit(524_288_000)]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Index([FromForm]int collectionYear, [FromForm]int period, IFormFile file, CancellationToken cancellationToken)
        {
            if (file != null)
            {
                await _periodEndService.SubmitJob(period, CollectionNames.ALLFCollection, User.Name(), User.Email(), file, cancellationToken);
            }

            return RedirectToAction("Index", new { collectionYear, period });
        }

        private async Task<IEnumerable<FileUploadJobMetaDataModel>> GetSubmittedFiles(int year, int period, CancellationToken cancellationToken)
        {
            return await _periodEndService.GetSubmissionsPerPeriodAsync(year, period, cancellationToken);
        }
    }
}