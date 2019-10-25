using System.Threading.Tasks;
using ESFA.DC.Web.Operations.Areas.PeriodEnd.Models;
using ESFA.DC.Web.Operations.Interfaces.PeriodEnd;
using ESFA.DC.Web.Operations.Utils;
using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.Web.Operations.Areas.PeriodEnd.Controllers
{
    [Area(AreaNames.PeriodEnd)]
    [Route(AreaNames.PeriodEnd + "/periodEndHistory")]
    public class HistoryController : Controller
    {
        private readonly IPeriodService _periodService;
        private readonly IHistoryService _historyService;

        public HistoryController(
            IPeriodService periodService,
            IHistoryService historyService)
        {
            _periodService = periodService;
            _historyService = historyService;
        }

        public async Task<IActionResult> Index(int? collectionYear)
        {
            var currentYearPeriod = await _periodService.ReturnPeriod();
            var model = new HistoryViewModel
            {
                Year = collectionYear ?? currentYearPeriod.Year ?? 0
            };

            model.PeriodHistories = await _historyService.GetHistoryDetails(model.Year);
            model.CollectionYears = await _historyService.GetCollectionYears();

            return View(model);
        }
    }
}