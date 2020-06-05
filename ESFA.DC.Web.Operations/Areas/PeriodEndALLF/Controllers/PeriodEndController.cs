using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Areas.PeriodEndALLF.Models;
using ESFA.DC.Web.Operations.Constants;
using ESFA.DC.Web.Operations.Extensions;
using ESFA.DC.Web.Operations.Interfaces.PeriodEnd;
using ESFA.DC.Web.Operations.Interfaces.Storage;
using ESFA.DC.Web.Operations.Models;
using ESFA.DC.Web.Operations.Utils;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.Web.Operations.Areas.PeriodEndALLF.Controllers
{
    [Area(AreaNames.PeriodEndALLF)]
    [Route(AreaNames.PeriodEndALLF + "/periodEnd")]
    public class PeriodEndController : BaseALLFController
    {
        private readonly IALLFPeriodEndService _periodEndService;
        private readonly IPeriodService _periodService;
        private readonly IStateService _stateService;

        public PeriodEndController(
            IALLFPeriodEndService periodEndService,
            IPeriodService periodService,
            IStateService stateService,
            IStorageService storageService,
            ILogger logger,
            TelemetryClient telemetryClient)
            : base(storageService, logger, telemetryClient)
        {
            _periodEndService = periodEndService;
            _periodService = periodService;
            _stateService = stateService;
        }

        [HttpGet("{collectionYear?}/{period?}")]
        public async Task<IActionResult> Index(int? collectionYear, int? period)
        {
            var cancellationToken = CancellationToken.None;

            var currentYearPeriod = await _periodService.ReturnPeriod(CollectionTypes.ALLF, cancellationToken);
            currentYearPeriod.Year = currentYearPeriod.Year ?? 0;

            PeriodEndViewModel model;

            if (collectionYear != null && period != null)
            {
                model = await ShowPath(collectionYear.Value, period.Value, cancellationToken);
                model.Files = (await GetSubmittedFiles(collectionYear.Value, period.Value, cancellationToken)).ToList();
            }
            else
            {
                model = await ShowPath(currentYearPeriod.Year.Value, currentYearPeriod.Period, cancellationToken);
                model.Files = (await GetSubmittedFiles(currentYearPeriod.Year.Value, currentYearPeriod.Period, cancellationToken)).ToList();
            }

            var isCurrentPeriodSelected = currentYearPeriod.Year == model.Year && currentYearPeriod.Period == model.Period;
            model.IsCurrentPeriod = isCurrentPeriodSelected;
            model.CollectionClosed = isCurrentPeriodSelected && currentYearPeriod.PeriodClosed;

            return View(model);
        }

        [RequestSizeLimit(524_288_000)]
        [AutoValidateAntiforgeryToken]
        [HttpPost("{collectionYear?}/{period?}")]
        public async Task<IActionResult> Index([FromForm]int collectionYear, [FromForm]int period, IFormFile file)
        {
            var cancellationToken = CancellationToken.None;
            if (file != null)
            {
                await _periodEndService.InitialisePeriodEndAsync(collectionYear, period, CollectionTypes.ALLF, cancellationToken);

                await _periodEndService.SubmitJob(period, CollectionNames.ALLFCollection, User.Name(), User.Email(), file, cancellationToken);
            }

            return RedirectToAction("Index", new { collectionYear, period });
        }

        private async Task<IEnumerable<FileUploadJobMetaDataModel>> GetSubmittedFiles(int year, int period, CancellationToken cancellationToken)
        {
            return await _periodEndService.GetSubmissionsPerPeriodAsync(year, period, false, cancellationToken);
        }

        private async Task<PeriodEndViewModel> ShowPath(int collectionYear, int period, CancellationToken cancellationToken)
        {
            var pathItemStates = await _periodEndService.GetPathItemStatesAsync(collectionYear, period, CollectionTypes.ALLF, cancellationToken);
            var state = _stateService.GetMainState(pathItemStates);
            var lastItemJobsFinished = _stateService.AllJobsHaveCompleted(state);

            var pathModel = new PeriodEndViewModel
            {
                Period = period,
                Year = collectionYear,
                Paths = state.Paths,
                PeriodEndStarted = state.PeriodEndStarted,
                PeriodEndFinished = state.PeriodEndFinished,
                ClosePeriodEndEnabled = lastItemJobsFinished
            };

            return pathModel;
        }
    }
}