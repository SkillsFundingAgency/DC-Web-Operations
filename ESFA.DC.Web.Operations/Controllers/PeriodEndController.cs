using System.Threading.Tasks;
using ESFA.DC.Web.Operations.Interfaces.PeriodEnd;
using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.Web.Operations.Controllers
{
    [Route("periodEnd")]
    public class PeriodEndController : Controller
    {
        private readonly IPeriodEndService _periodEndService;

        public PeriodEndController(IPeriodEndService periodEndService)
        {
            _periodEndService = periodEndService;
        }

        public async Task<IActionResult> Index()
        {
            var paths = await _periodEndService.GetPathItemStates(1819, 1);
            ViewBag.Paths = paths;
            return View();
        }

        [HttpPost("startPeriodEnd")]
        public IActionResult StartPeriodEnd()
        {
            _periodEndService.StartPeriodEnd(1819, 1);

            return View("Index");
        }

        [HttpPost("proceed")]
        public IActionResult Proceed()
        {
            _periodEndService.Proceed(1819, 1, 0);

            return View("Index");
        }
    }
}