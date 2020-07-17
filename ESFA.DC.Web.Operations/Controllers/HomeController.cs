using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.Collections;
using ESFA.DC.Web.Operations.Interfaces.Dashboard;
using ESFA.DC.Web.Operations.Interfaces.PeriodEnd;
using ESFA.DC.Web.Operations.Settings.Models;
using ESFA.DC.Web.Operations.Utils;
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
        private readonly IPeriodService _periodService;
        private readonly ICollectionsService _collectionsService;

        public HomeController(
            IDashBoardService dashBoardService,
            ILogger logger,
            IPeriodService periodService,
            ICollectionsService collectionService,
            TelemetryClient telemetryClient,
            IHttpContextAccessor httpContextAccessor,
            AuthorizationSettings authorizationSettings)
            : base(logger, telemetryClient)
        {
            _dashBoardService = dashBoardService;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _authorizationSettings = authorizationSettings;
            _periodService = periodService;
            _collectionsService = collectionService;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var stats = _dashBoardService.GetStatsAsync(cancellationToken);
            var currentYearPeriod = _periodService.ReturnPeriod(CollectionTypes.ILR, cancellationToken);

            await Task.WhenAll(stats, currentYearPeriod);

            var model = new HomeViewModel
            {
                DashboardStats = stats.Result,
                CollectionYear = currentYearPeriod.Result.Year.GetValueOrDefault(),
                CollectionYears = new List<int> { 1920, 2021 } //TODO How to get?
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
