using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.Dashboard;
using ESFA.DC.Web.Operations.Settings.Models;
using ESFA.DC.Web.Operations.ViewModels;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.Web.Operations.Controllers
{
    public class HomeController : BaseController
    {
        private readonly IDashBoardService _dashBoardService;
        private readonly ILogger _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AuthorizationSettings _authorizationSettings;

        public HomeController(
            IDashBoardService dashBoardService,
            ILogger logger,
            TelemetryClient telemetryClient,
            IHttpContextAccessor httpContextAccessor,
            AuthorizationSettings authorizationSettings)
            : base(logger, telemetryClient)
        {
            _dashBoardService = dashBoardService;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _authorizationSettings = authorizationSettings;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var model = new HomeViewModel
            {
                DashboardStats = await _dashBoardService.GetStatsAsync(cancellationToken)
            };

            return View("Index", model);
        }

        [Route("/NotAuthorized")]
        public IActionResult NotAuthorized()
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
