using System;
using System.Threading.Tasks;
using ESFA.DC.Web.Operations.Interfaces.PeriodEnd;
using ESFA.DC.Web.Operations.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.Web.Operations.Controllers
{
    [Route("periodEnd")]
    public class PeriodEndController : Controller
    {
        private readonly IPeriodService _periodService;
        private readonly IPeriodEndService _periodEndService;

        public PeriodEndController(
            IPeriodService periodService,
            IPeriodEndService periodEndService)
        {
            _periodService = periodService;
            _periodEndService = periodEndService;
        }

        [HttpGet("{collectionYear?}/{period?}")]
        public async Task<IActionResult> Index(int? collectionYear, int? period)
        {
            var currentYearPeriod = await _periodService.ReturnPeriod(DateTime.UtcNow);
            PeriodEndViewModel model;

            if (collectionYear != null && period != null)
            {
                model = await ShowPath(collectionYear.Value, period.Value);
            }
            else
            {
                model = await ShowPath(currentYearPeriod.Year, currentYearPeriod.Period);
            }

            model.CurrentPeriod = currentYearPeriod.Period;

            return View(model);
        }

        [HttpPost("startPeriodEnd")]
        public async Task<IActionResult> StartPeriodEnd(int collectionYear, int period)
        {
            await _periodEndService.StartPeriodEnd(collectionYear, period);

            var currentYearPeriod = await _periodService.ReturnPeriod(DateTime.UtcNow);
            var model = await ShowPath(collectionYear, period);
            model.CurrentPeriod = currentYearPeriod.Period;

            return View("Index", model);
        }

        [HttpPost("proceed")]
        public async Task<IActionResult> Proceed(int collectionYear, int period)
        {
            await _periodEndService.Proceed(collectionYear, period, 0);

            var currentYearPeriod = await _periodService.ReturnPeriod(DateTime.UtcNow);
            var model = await ShowPath(collectionYear, period);
            model.CurrentPeriod = currentYearPeriod.Period;

            return View("Index", model);
        }

        [HttpPost("selectPeriod")]
        public async Task<IActionResult> SelectPeriod(int collectionYear, int period)
        {
            return RedirectToAction("Index", new { collectionYear, period });
        }

        private async Task<PeriodEndViewModel> ShowPath(int collectionYear, int period)
        {
            var paths = await _periodEndService.GetPathItemStates(collectionYear, period);

            var model = new PeriodEndViewModel
            {
                Period = period,
                Year = collectionYear,
                Paths = paths
            };

            return model;
        }
    }
}