using System;
using System.Threading.Tasks;
using ESFA.DC.Web.Operations.Interfaces.PeriodEnd;
using ESFA.DC.Web.Operations.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.Web.Operations.Controllers
{
    [Route("periodEndPreparation")]
    public class PeriodEndPrepController : Controller
    {
        private readonly IPeriodService _periodService;
        private readonly IPeriodEndService _periodEndService;

        public PeriodEndPrepController(
            IPeriodService periodService,
            IPeriodEndService periodEndService)
        {
            _periodService = periodService;
            _periodEndService = periodEndService;
        }

        public async Task<IActionResult> Index(int? collectionYear, int? period)
        {
            var currentYearPeriod = await _periodService.ReturnPeriod(DateTime.UtcNow);
            var model = new PeriodEndPrepViewModel();

            if (collectionYear != null && period != null)
            {
                model.Year = collectionYear.Value;
                model.Period = period.Value;
            }
            else
            {
                model.Year = currentYearPeriod.Year;
                model.Period = currentYearPeriod.Period;
            }

            model.FailedJobs = await GetFailedJobs(model.Year, model.Period);
            model.ReferenceDataJobs = await GetReferenceDataJobs();

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

        [HttpPost("pauseReferenceData")]
        public async Task<IActionResult> PauseReferenceJobs(int collectionYear, int period)
        {
            await _periodEndService.ToggleReferenceDataJobs(true);

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
            var data = await _periodEndService.GetFailedJobs("ILR", collectionYear, period);

            return data;
        }
    }
}