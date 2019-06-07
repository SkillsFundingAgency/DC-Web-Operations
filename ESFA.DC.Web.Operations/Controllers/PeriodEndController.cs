using ESFA.DC.Web.Operations.Interfaces.PeriodEnd;
using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.Web.Operations.Controllers
{
    public class PeriodEndController : Controller
    {
        private readonly IPeriodEndService _periodEndService;

        public PeriodEndController(IPeriodEndService periodEndService)
        {
            _periodEndService = periodEndService;
        }

        public IActionResult Index()
        {
            _periodEndService.Proceed();

            return View();
        }
    }
}