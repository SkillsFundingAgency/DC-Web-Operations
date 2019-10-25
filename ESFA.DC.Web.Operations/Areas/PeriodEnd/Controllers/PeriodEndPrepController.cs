using System.Threading.Tasks;
using ESFA.DC.Web.Operations.Areas.PeriodEnd.Models;
using ESFA.DC.Web.Operations.Interfaces.PeriodEnd;
using ESFA.DC.Web.Operations.Utils;
using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.Web.Operations.Areas.PeriodEnd.Controllers
{
    [Area(AreaNames.PeriodEnd)]
    [Route(AreaNames.PeriodEnd + "/periodEndPreparation")]
    public class PeriodEndPrepController : Controller
    {
        private readonly IPeriodService _periodService;
        private readonly IPeriodEndService _periodEndService;
        private readonly IStateService _stateService;

        public PeriodEndPrepController(
            IPeriodService periodService,
            IPeriodEndService periodEndService,
            IStateService stateService)
        {
            _periodService = periodService;
            _periodEndService = periodEndService;
            _stateService = stateService;
        }

        [HttpGet("{collectionYear?}/{period?}")]
        public async Task<IActionResult> Index(int? collectionYear, int? period)
        {
            var currentYearPeriod = await _periodService.ReturnPeriod();
            currentYearPeriod.Year = currentYearPeriod.Year ?? 0;
            var model = new PeriodEndPrepViewModel();

            if (collectionYear != null && period != null)
            {
                model.Year = collectionYear.Value;
                model.Period = period.Value;
            }
            else
            {
                model.Year = currentYearPeriod.Year.Value;
                model.Period = currentYearPeriod.Period;
            }

            var isCurrentPeriodSelected = currentYearPeriod.Year == model.Year && currentYearPeriod.Period == model.Period;
            model.Closed = isCurrentPeriodSelected && currentYearPeriod.PeriodClosed;

            model.FailedJobs = await GetFailedJobs(model.Year, model.Period);
            model.ReferenceDataJobs = await GetReferenceDataJobs();

            var pathItemStates = await _periodEndService.GetPathItemStates(model.Year, model.Period);
            model.CollectionClosedEmailSent = _stateService.CollectionClosedEmailSent(pathItemStates);

            return View(model);
        }

        [HttpPost("selectPeriod")]
        public IActionResult SelectPeriod(int collectionYear, int period)
        {
            return RedirectToAction("Index", new { collectionYear, period });
        }

        [HttpPost("resubmitJob")]
        public async Task<IActionResult> ReSubmitJob(int collectionYear, int period, int jobId)
        {
            await _periodEndService.ReSubmitFailedJob(jobId);

            return RedirectToAction("Index", new { collectionYear, period });
        }

        [HttpPost("startPeriodEnd")]
        public IActionResult StartPeriodEnd(int collectionYear, int period)
        {
            return RedirectToAction("Index", "PeriodEnd", new { collectionYear, period });
        }

        private async Task<string> GetReferenceDataJobs()
        {
            var data = await _periodEndService.GetReferenceDataJobs();

            return data;
        }

        private async Task<string> GetFailedJobs(int collectionYear, int period)
        {
            var data = await _periodEndService.GetFailedJobs(collectionYear, period);

            return data;
        }
    }
}