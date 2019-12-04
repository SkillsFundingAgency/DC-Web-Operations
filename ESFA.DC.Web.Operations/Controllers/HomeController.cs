using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.Dashboard;
using ESFA.DC.Web.Operations.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.Web.Operations.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IDashBoardService _dashBoardService;
        private readonly ILogger _logger;

        public HomeController(
            IDashBoardService dashBoardService,
            ILogger logger)
        {
            _dashBoardService = dashBoardService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _dashBoardService.ProvideAsync(CancellationToken.None));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
