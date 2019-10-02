using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Web.Operations.Areas.PeriodEnd.Models;
using ESFA.DC.Web.Operations.Interfaces.PeriodEnd;
using ESFA.DC.Web.Operations.Utils;
using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.Web.Operations.Areas.PeriodEnd.Controllers
{
    [Area(AreaNames.PeriodEnd)]
    [Route(AreaNames.PeriodEnd + "/periodEnd")]
    public class PeriodEndController : Controller
    {
        private readonly IPeriodService _periodService;
        private readonly IPeriodEndService _periodEndService;
        private readonly IJsonSerializationService _jsonSerializationService;
        private readonly IEmailService _emailService;

        public PeriodEndController(
            IPeriodService periodService,
            IPeriodEndService periodEndService,
            IJsonSerializationService jsonSerializationService,
            IEmailService emailService)
        {
            _periodService = periodService;
            _periodEndService = periodEndService;
            _jsonSerializationService = jsonSerializationService;
            _emailService = emailService;
        }

        [HttpGet("{collectionYear?}/{period?}")]
        public async Task<IActionResult> Index(int? collectionYear, int? period)
        {
            var currentYearPeriod = await _periodService.ReturnPeriod();
            PeriodEndViewModel model;

            if (collectionYear != null && period != null)
            {
                model = await ShowPath(collectionYear.Value, period.Value);
                model.Year = collectionYear.Value;
                model.Period = period.Value;
            }
            else
            {
                model = await ShowPath(currentYearPeriod.Year, currentYearPeriod.Period);
                model.Year = currentYearPeriod.Year;
                model.Period = currentYearPeriod.Period;
            }

            var isCurrentPeriodSelected = currentYearPeriod.Year == model.Year && currentYearPeriod.Period == model.Period;
            model.Closed = isCurrentPeriodSelected && currentYearPeriod.PeriodClosed;

            return View(model);
        }

        [HttpPost("unPauseReferenceData")]
        public async Task<IActionResult> UnPauseReferenceJobs(int collectionYear, int period)
        {
            await _periodEndService.ToggleReferenceDataJobs(false);

            return RedirectToAction("Index", new { collectionYear, period });
        }

        [HttpPost("publishProviderReports")]
        public async Task<IActionResult> PublishProviderReports(int collectionYear, int period)
        {
            await _periodEndService.PublishProviderReports(collectionYear, period);

            await _emailService.SendEmail(EmailIds.ReportsPublishedEmail, period);

            return RedirectToAction("Index", new { collectionYear, period });
        }

        [HttpPost("publishMcaReports")]
        public async Task<IActionResult> PublishMcaReports(int collectionYear, int period)
        {
            await _periodEndService.PublishMcaReports(collectionYear, period);

            return RedirectToAction("Index", new { collectionYear, period });
        }

        [HttpPost("closePeriodEnd")]
        public async Task<IActionResult> ClosePeriodEnd(int collectionYear, int period)
        {
            await _periodEndService.ClosePeriodEnd(collectionYear, period);

            return RedirectToAction("Index", new { collectionYear, period });
        }

        private async Task<PeriodEndViewModel> ShowPath(int collectionYear, int period)
        {
            var paths = await _periodEndService.GetPathItemStates(collectionYear, period);

            var model = _jsonSerializationService.Deserialize<List<PeriodEndViewModel>>(paths).FirstOrDefault();

            var pathModel = new PeriodEndViewModel
            {
                Period = period,
                Year = collectionYear,
                Paths = paths,
                Closed = model?.Closed ?? false,
                ReportsPublished = model?.ReportsPublished ?? false
            };

            return pathModel;
        }
    }
}