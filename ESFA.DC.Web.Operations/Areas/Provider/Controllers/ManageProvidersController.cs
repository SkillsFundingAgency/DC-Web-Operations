using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Areas.Provider.Models;
using ESFA.DC.Web.Operations.Controllers;
using ESFA.DC.Web.Operations.Interfaces.Provider;
using ESFA.DC.Web.Operations.Utils;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.Web.Operations.Areas.Provider.Controllers
{
    [Area(AreaNames.Provider)]
    public class ManageProvidersController : BaseControllerWithDevOpsOrAdvancedSupportPolicy
    {
        private readonly IManageProvidersService _manageProvidersService;

        public ManageProvidersController(ILogger logger, IManageProvidersService manageProvidersService, TelemetryClient telemetryClient)
            : base(logger, telemetryClient)
        {
            _manageProvidersService = manageProvidersService;
        }

        public async Task<IActionResult> Index(long ukprn, CancellationToken cancellationToken)
        {
            var viewModel = new ManageProviderViewModel();
            var provider = await _manageProvidersService.GetProviderAsync(ukprn, cancellationToken);
            viewModel.CollectionAssignments = await _manageProvidersService.GetProviderAssignmentsAsync(ukprn, cancellationToken);

            viewModel.ProviderName = provider.Name;
            viewModel.Ukprn = provider.Ukprn;
            viewModel.Upin = provider.Upin;
            viewModel.IsMca = provider.IsMca.GetValueOrDefault();

            return View("Index", viewModel);
        }
    }
}