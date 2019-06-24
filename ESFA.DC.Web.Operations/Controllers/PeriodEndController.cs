using ESFA.DC.Web.Operations.Interfaces.PeriodEnd;
using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.Web.Operations.Controllers
{
    [Route("periodend")]
    public class PeriodEndController : Controller
    {
        private readonly IPeriodEndService _periodEndService;

        public PeriodEndController(IPeriodEndService periodEndService)
        {
            _periodEndService = periodEndService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("proceed")]
        public IActionResult Proceed()
        {
            _periodEndService.Proceed();

            return View("Index");
        }
    }
}